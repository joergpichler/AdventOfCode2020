using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Lib;

namespace Day22
{
    class Program
    {
        static void Main(string[] args)
        {
            var stringBuilder = new StringBuilder();
            Assembly.GetExecutingAssembly().GetEmbeddedResourceLines("Day22.input.txt")
                .ForEach(line => stringBuilder.AppendLine(line));

            var input = ParseInput(stringBuilder.ToString());

            Part1(input.deckA, input.deckB);

            Console.WriteLine();

            input = ParseInput(stringBuilder.ToString());

            Part2(input.deckA, input.deckB);
        }

        private static void Part1(Deck deckA, Deck deckB)
        {
            var game = new CardGame(deckA, deckB);
            var winningDeck = game.Play();
            
            ConsoleHelper.Part1();
            Console.WriteLine($"Score: {winningDeck.GetScore()}");
        }

        private static void Part2(Deck deckA, Deck deckB)
        {
            var game = new RecursiveCombatCardGame(deckA, deckB);
            var winningDeck = game.Play();

            ConsoleHelper.Part2();
            Console.WriteLine($"Score: {winningDeck.GetScore()}");
        }

        private static (Deck deckA, Deck deckB) ParseInput(string input)
        {
            var matches = Regex.Matches(input, "Player\\s\\d:(?:\\r\\n+?(?<card>\\d+))*", RegexOptions.Compiled | RegexOptions.Singleline);
            if (matches.Count != 2)
            {
                throw new InvalidOperationException();
            }

            Deck ParseMatch(Match match) => new(match.Groups["card"].Captures.Select(c => new Card(int.Parse(c.Value))));

            var deckA = ParseMatch(matches[0]);
            var deckB = ParseMatch(matches[1]);

            return (deckA, deckB);
        }
    }

    class RecursiveCombatCardGame
    {
        private readonly Deck _deckA;
        private readonly Deck _deckB;
        private readonly HashSet<int> _deckAHashes = new();
        private readonly HashSet<int> _deckBHashes = new();

        public RecursiveCombatCardGame(Deck deckA, Deck deckB)
        {
            _deckA = deckA;
            _deckB = deckB;
        }

        public Deck Play()
        {
            int round = 0;

            while (_deckA.CardCount > 0 && _deckB.CardCount > 0)
            {
                round += 1;

                if (_deckAHashes.Contains(_deckA.GetHashCode()) && _deckBHashes.Contains(_deckB.GetHashCode()))
                {
                    return _deckA;
                }

                _deckAHashes.Add(_deckA.GetHashCode());
                _deckBHashes.Add(_deckB.GetHashCode());

                var cardA = _deckA.DrawCard();
                var cardB = _deckB.DrawCard();

                if (_deckA.CardCount >= cardA.Value && _deckB.CardCount >= cardB.Value)
                {
                    var subDeckA = _deckA.Clone(cardA.Value);
                    var subDeckB = _deckB.Clone(cardB.Value);

                    var subGame = new RecursiveCombatCardGame(subDeckA, subDeckB);
                    var winningDeck = subGame.Play();

                    if (winningDeck == subDeckA)
                    {
                        _deckA.AddCards(cardA, cardB);
                    }
                    else
                    {
                        _deckB.AddCards(cardB, cardA);
                    }
                }
                else
                {
                    if (cardA.Value > cardB.Value)
                    {
                        _deckA.AddCards(cardA, cardB);
                    }
                    else if (cardB.Value > cardA.Value)
                    {
                        _deckB.AddCards(cardB, cardA);
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }
                }
            }

            return _deckA.CardCount > 0 ? _deckA : _deckB;
        }
    }

    class CardGame
    {
        private readonly Deck _deckA;
        private readonly Deck _deckB;

        public CardGame(Deck deckA, Deck deckB)
        {
            _deckA = deckA;
            _deckB = deckB;
        }

        public bool Debug { get; set; }

        public Deck Play()
        {
            int round = 0;
            
            while (_deckA.CardCount > 0 && _deckB.CardCount > 0)
            {
                round += 1;

                if (Debug)
                {
                    Console.WriteLine($"-- Round {round} --");
                    Console.WriteLine($"Player 1's deck: {_deckA}");
                    Console.WriteLine($"Player 2's deck: {_deckB}");
                }
                
                var cardA = _deckA.DrawCard();
                var cardB = _deckB.DrawCard();

                if (Debug)
                {
                    Console.WriteLine($"Player 1 plays: {cardA.Value}");
                    Console.WriteLine($"Player 2 plays: {cardB.Value}");
                }

                if (cardA.Value > cardB.Value)
                {
                    _deckA.AddCards(cardA, cardB);
                    if (Debug)
                    {
                        Console.WriteLine("Player 1 wins the round!");
                    }
                }
                else if (cardB.Value > cardA.Value)
                {
                    _deckB.AddCards(cardB, cardA);
                    if (Debug)
                    {
                        Console.WriteLine("Player 2 wins the round!");
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }

                if (Debug)
                {
                    Console.WriteLine();
                    Console.ReadLine();
                }
            }

            return _deckA.CardCount > 0 ? _deckA : _deckB;
        }
    }

    class Deck
    {
        private readonly Queue<Card> _cards;

        public Deck(IEnumerable<Card> cards)
        {
            _cards = new Queue<Card>(cards);
        }

        public int CardCount => _cards.Count;
        
        public Card DrawCard()
        {
            return _cards.Dequeue();
        }

        public void AddCards(params Card[] cards)
        {
            cards.ForEach(c => _cards.Enqueue(c));
        }

        public int GetScore()
        {
            int multiplier = _cards.Count;
            int score = 0;
            
            _cards.ForEach(c => score += multiplier-- * c.Value);

            return score;
        }

        public Deck Clone(int cardCount)
        {
            return new Deck(_cards.Take(cardCount));
        }

        public override string ToString()
        {
            return _cards.Select(c => c.Value.ToString()).Aggregate((a, b) => a + ", " + b);
        }

        public override int GetHashCode()
        {
            return _cards.Select((card, index) => HashCode.Combine(index, card.Value))
                .Aggregate((a, b) => HashCode.Combine(a, b));
        }
    }
    
    [DebuggerDisplay("{Value}")]
    class Card
    {
        public Card(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }
}
