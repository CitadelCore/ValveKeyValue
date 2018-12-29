using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ValveKeyValue
{
    internal class KvConditionEvaluator
    {
        public KvConditionEvaluator(ICollection<string> definedVariables)
        {
            Require.NotNull(definedVariables, nameof(definedVariables));
            _definedVariables = definedVariables;
        }

        private readonly ICollection<string> _definedVariables;

        public bool Evalute(string expressionText)
        {
            Expression expression;
            try
            {
                var tokens = new List<KvConditionToken>();
                using (var reader = new StringReader(expressionText))
                {
                    KvConditionToken token;
                    while ((token = ReadToken(reader)) != null)
                        tokens.Add(token);
                }

                expression = CreateExpression(tokens);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidDataException($"Invalid conditional syntax \"{expressionText}\"", ex);
            }

            var value = (bool)Expression.Lambda(expression).Compile().DynamicInvoke();
            return value;
        }

        private bool EvaluateVariable(string variable) => _definedVariables.Contains(variable);

        private Expression CreateExpression(IList<KvConditionToken> tokens)
        {
            if (tokens.Count == 0)
            {
                throw new InvalidOperationException($"{nameof(CreateExpression)} called with no condition tokens.");
            }

            PreprocessBracketedExpressions(tokens);
            KvConditionToken token;

            // Process AND and OR next. Split the list of expressions into two parts, and recursively process
            // each part before joining in the desired expression.
            for (var i = 0; i < tokens.Count; i++)
            {
                token = tokens[i];
                if (token.TokenType != KvConditionTokenType.OrJoin && token.TokenType != KvConditionTokenType.AndJoin)
                {
                    continue;
                }

                var left = tokens.Take(i).ToList();
                var right = tokens.Skip(i + 1).ToList();

                var leftExpression = CreateExpression(left);
                var rightExpression = CreateExpression(right);

                switch (token.TokenType)
                {
                    case KvConditionTokenType.OrJoin:
                        return Expression.OrElse(leftExpression, rightExpression);
                    case KvConditionTokenType.AndJoin:
                        return Expression.AndAlso(leftExpression, rightExpression);
                }
            }

            if (tokens.Count == 2 && tokens[0].TokenType == KvConditionTokenType.Negation)
            {
                var positiveExpression = CreateExpression(tokens.Skip(1).ToList());
                return Expression.Not(positiveExpression);
            }
            else if (tokens.Count == 1)
            {
                token = tokens.Single();
                switch (token.TokenType)
                {
                    case KvConditionTokenType.Value:
                        return EvaluteVariableExpression((string)token.Value);
                    case KvConditionTokenType.PreprocessedExpression:
                        return (Expression)token.Value;
                }
            }

            throw new InvalidOperationException("Invalid conditional syntax.");
        }

        private void PreprocessBracketedExpressions(IList<KvConditionToken> tokens)
        {
            int startIndex;
            for (startIndex = 0; startIndex < tokens.Count; startIndex++)
            {
                if (tokens[startIndex].TokenType == KvConditionTokenType.BeginSubExpression)
                    break;
            }

            if (startIndex == tokens.Count)
                return;

            int endIndex;
            for (endIndex = tokens.Count - 1; endIndex > startIndex; endIndex--)
            {
                if (tokens[endIndex].TokenType == KvConditionTokenType.EndSubExpression)
                    break;
            }

            if (endIndex == 0)
                return;

            var subRange = tokens.Skip(startIndex + 1).Take(endIndex - startIndex - 1).ToList();
            var evalutedExpression = CreateExpression(subRange);

            for (var i = 0; i < endIndex - startIndex + 1; i++)
            {
                tokens.RemoveAt(startIndex);
            }

            tokens.Insert(startIndex, new KvConditionToken(evalutedExpression));
        }

        private Expression EvaluteVariableExpression(string variable)
        {
            var instance = Expression.Constant(this);
            var method = typeof(KvConditionEvaluator)
                .GetMethod(nameof(EvaluateVariable), BindingFlags.NonPublic | BindingFlags.Instance);
            return Expression.Call(instance, method ?? throw new InvalidOperationException(), Expression.Constant(variable));
        }

        private static KvConditionToken ReadToken(TextReader reader)
        {
            SkipWhitespace(reader);

            var current = reader.Read();

            switch (current)
            {
                case -1:
                    return null; // End of string
                case '$':
                    return ReadVariableToken(reader);
                case '!':
                    return KvConditionToken.Not;
                case '|':
                {
                    var next = reader.Peek();
                    if (next != -1 && (char)next == '|')
                    {
                        reader.Read();
                        return KvConditionToken.Or;
                    }

                    break;
                }
                case '&':
                {
                    var next = reader.Peek();
                    if (next != -1 && (char)next == '&')
                    {
                        reader.Read();
                        return KvConditionToken.And;
                    }

                    break;
                }
                case '(':
                    return KvConditionToken.LeftParenthesis;
                case ')':
                    return KvConditionToken.RightParenthesis;
            }

            throw new InvalidOperationException("Bad condition syntax.");
        }

        private static void SkipWhitespace(TextReader reader)
        {
            var next = reader.Peek();
            while (next != -1 && char.IsWhiteSpace((char)next))
            {
                reader.Read();
                next = reader.Peek();
            }
        }

        private static KvConditionToken ReadVariableToken(TextReader reader)
        {
            var builder = new StringBuilder();
            while (IsReadableVariableCharacter(reader.Peek()))
                builder.Append((char)reader.Read());

            return new KvConditionToken(builder.ToString());
        }

        private static bool IsReadableVariableCharacter(int value)
        {
            return value != -1 && char.IsLetterOrDigit((char)value);
        }

        private enum KvConditionTokenType
        {
            Value,
            Negation,
            OrJoin,
            AndJoin,
            BeginSubExpression,
            EndSubExpression,
            PreprocessedExpression // Used internally for bracketed expressions
        }

        private class KvConditionToken
        {
            public KvConditionToken(string variable)
                : this(KvConditionTokenType.Value)
            {
                Value = variable;
            }

            public KvConditionToken(Expression expression)
                 : this(KvConditionTokenType.PreprocessedExpression)
            {
                Value = expression;
            }

            private KvConditionToken(KvConditionTokenType type)
            {
                TokenType = type;
            }

            public KvConditionTokenType TokenType { get; }

            public object Value { get; }

            public static KvConditionToken Not
                => new KvConditionToken(KvConditionTokenType.Negation);

            public static KvConditionToken Or
                => new KvConditionToken(KvConditionTokenType.OrJoin);

            public static KvConditionToken And
                => new KvConditionToken(KvConditionTokenType.AndJoin);

            public static KvConditionToken LeftParenthesis
                => new KvConditionToken(KvConditionTokenType.BeginSubExpression);

            public static KvConditionToken RightParenthesis
                => new KvConditionToken(KvConditionTokenType.EndSubExpression);
        }
    }
}
