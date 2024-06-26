﻿// See https://aka.ms/new-console-template for more information
using System;

namespace compcour{
    class Program{
        static void Main(string[] args){
            Console.Write("Hello, World!");
            while(true){
                Console.Write("\n\n> ");
                Console.Write("Enter the string: ");
                var line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line)) return;

                var lexer = new Lexer(line);
                while(true){
                    var token = lexer.NextToken();
                    if (token.Kind == SyntaxKind.EndOfFileToken) break;

                    Console.Write($"{token.Kind}: '{token.Text}'");

                    // if (token.Value != null) Console.Write($"{token.Value}");

                    Console.WriteLine();
                }

                // if (line == "1 + 2 * 3") Console.Write("7");
                // else{
                //     Console.Write("\n> ERROR: Invalid Expression\n");
                // }
            }
        }
    }

    enum SyntaxKind{
        NumberToken,
        WhiteSpaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,
        NumberExpression,
        BinaryExpression
    }

    class SyntaxToken{
        public SyntaxToken(SyntaxKind kind, int position, string text, object value){
            // $ function to create a new syntax token
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }
        public SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }
    }

    class Lexer{

        private readonly string _text;
        public Lexer(string text){
            _text = text;
        }
        private int _position;

        private char Current{
            get{
                if (_position >= _text.Length) return '\0';
                return _text[_position];
            }
        }

        private void Next(){
            _position++;
        }

        public SyntaxToken NextToken(){

            // $ NUMBERS: + - * / whitespace ( )

            if(_position >= _text.Length){
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
            }

            if(char.IsDigit(Current)){
                var start = _position;

                while (char.IsDigit(Current)) Next();

                var length = _position - start;

                var text = _text.Substring(start, length);
                int.TryParse(text, out var value);
                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if(char.IsWhiteSpace(Current)){
                var start = _position;

                while (char.IsWhiteSpace(Current)) Next();

                var length = _position - start;

                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null);
            }

            if (Current == '+') return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
            else if (Current == '-') return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
            else if (Current == '*') return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
            else if (Current == '/') return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
            else if (Current == '(') return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
            else if (Current == ')') return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);

            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }

    }

    // $ Syntax Tree
    // $ Done to represent the structure of the program
    abstract class SyntaxNode{
        public abstract SyntaxKind Kind { get; }
    }

    // $ Represents a single expression
    abstract class ExpressionSyntax : SyntaxNode{

    }

    // $ Represents a single number expression
    sealed class NumberExpressionSyntax(SyntaxToken numberToken) : ExpressionSyntax{
        public override SyntaxKind Kind => SyntaxKind.NumberExpression;
        public SyntaxToken NumberToken { get; } = numberToken;
    }

    // $ Represents a binary expression
    sealed class BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right) : ExpressionSyntax{
        public ExpressionSyntax Left { get; } = left;
        public SyntaxToken OperatorToken { get; } = operatorToken;
        public ExpressionSyntax Right { get; } = right;
        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
    }

    class Parser{

        private readonly SyntaxToken[] _tokens;
        private int _position;
        public Parser(string text){
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;
            do {

                token = lexer.NextToken();

                if(token.Kind != SyntaxKind.BadToken && token.Kind != SyntaxKind.WhiteSpaceToken){
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);
            _tokens = tokens.ToArray();
        }

        private SyntaxToken Peek(int offset){
            var index = _position + offset;
            if (index >= _tokens.Length) return _tokens[_tokens.Length - 1];
            return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);
        // $ Helps to move to the next token
    }
}


