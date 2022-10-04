using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO.Pipes;
using System.Threading.Channels;


namespace SOLID.Princeples;

public class DIP
{
    public class DurakGame
    {
        public enum Suit
        {
            clubs = 0,
            diamonds = 1,
            hearts = 2,
            sprades = 3
        }


        public enum Rank
        {
            Ace = 8,
            King = 7,
            Queen = 6,
            Jack = 5,
            C10 = 4,
            C9 = 3,
            C8 = 2,
            C7 = 1,
            C6 = 0
        }


        public record Card(Rank Rank, Suit Suit)
        {
            public override string ToString() => $"({Rank} {Suit})";
        }


        public abstract class AbstractPlayer
        {
            #pragma warning disable CS0067
            public event Action<AbstractPlayer> WantsPass, AcceptDefeat;
            #pragma warning restore CS0067


            public event Action<AbstractPlayer, Card, int> WantsThrowCard;


            public event Action<AbstractPlayer, Card> OnCardAdded;


            protected readonly List<Card> _cards;


            public string Name { get; init; }


            protected AbstractPlayer(string name)
            {
                Name = name;
                _cards = new List<Card>();
            }


            public virtual void AddCard(Card card)
            {
                _cards.Add(card);
                OnCardAdded?.Invoke(this, card);
            }


            public virtual void RemoveCard(Card card)
            {
                if (!_cards.Contains(card))
                {
                    throw new ArgumentException();
                }

                _cards.Remove(card);
            }


            public abstract void NotifyNextRound(Round round);


            public abstract void NotifyPlayerThrowsCard(AbstractPlayer player, Card card, int slotIndex);


            public abstract void NotifyGameEnded(GameEndResult gameEndResult);


            protected void WantThrowCard(Card card, int place)
            {
                WantsThrowCard?.Invoke(this, card, place);
            }


            protected void WantPass()
            {
                WantsPass?.Invoke(this);
            }
        }


        public class BotPlayer : AbstractPlayer
        {
            private Random random;


            public BotPlayer(string name) : base(name)
            {
                random = new Random(DateTime.Now.Millisecond);
            }


            private Round currentRound;


            private Card? GetMyRandomCardOrNull => _cards.Count > 0 ? _cards[random.Next(0, _cards.Count)] : null;


            public override void NotifyNextRound(Round round)
            {
                currentRound = round;

                if (round.Attacker == this)
                {
                    var card = GetMyRandomCardOrNull ?? throw new Exception();

                    WantThrowCard(card, 0);
                }
            }


            public override void NotifyPlayerThrowsCard(AbstractPlayer player, Card card, int slotIndex)
            {
                if (player == null)
                {
                    throw new ArgumentNullException(nameof(player));
                }

                if (player == this)
                    return;

                if (player == currentRound.Defender)
                {
                    WantPass();
                }
            }


            public override void NotifyGameEnded(GameEndResult gameEndResult) { }
        }


        public class ConsolePlayer : AbstractPlayer
        {
            public ConsolePlayer(string name) : base(name) { }


            public override void NotifyNextRound(Round round)
            {
                
                
                Print($"Round started\nAttacker: {round.Attacker.Name}\nDefender: {round.Defender.Name}");


                if (round.Attacker == this)
                {
                    DoTurn();
                }
            }


            public override void NotifyPlayerThrowsCard(AbstractPlayer player, Card card, int slotIndex)
            {
                Print($"{player.Name} throws card {card} on place {slotIndex}");
            }


            public override void NotifyGameEnded(GameEndResult gameEndResult)
            {
                Print(
                    $"Game ended! Winners:\n{String.Join('\n', gameEndResult.Winners.Select(player => player.Name))}\n");
            }


            private void DoTurn()
            {
                Print($"Please do turn\n{string.Join(' ', _cards)}\n0 = pass, 1-6 choice card");

                var index = int.Parse(Console.ReadLine());

                if (index == 0)
                {
                    WantPass();
                }
                else
                {
                    WantThrowCard(_cards[--index], 0);
                }
            }


            private void Print(string content)
            {
                Console.WriteLine(content);
            }
        }


        public class Judge
        {
            private readonly Card trump;


            public Judge(Card trump)
            {
                this.trump = trump;
            }


            public bool IsTrump(Card card) => card.Suit == trump.Suit;


            public Card GetStronger(Card c1, Card c2)
            {
                if (!IsTrump(c1) && !IsTrump(c2) && c1.Suit != c2.Suit)
                {
                    throw new ArgumentException();
                }

                if ((IsTrump(c1) && IsTrump(c2))
                    || (!IsTrump(c1) && !IsTrump(c2)))
                {
                    return c1.Rank > c2.Rank ? c1 : c2;
                }

                return IsTrump(c1) ? c1 : c2;
            }
        }


        public class Table
        {
            private readonly Judge judge;
            public readonly IReadOnlyList<Slot> slots;


            public Table(Judge judge, in int slotsCount = 6)
            {
                this.judge = judge;
                var slotList = new List<Slot>();
                for (int i = 0; i < slotsCount; i++)
                {
                    slotList.Add(new Slot());
                }

                slots = slotList;
            }


            public bool IsAllAttackingCardsCovered()
            {
                foreach (var slot in slots)
                {
                    if (slot.AttackingCard != null && slot.DefendingCard == null)
                        return false;
                }

                return true;
            }


            public bool IsAllSlotsFree()
            {
                foreach (var slot in slots)
                {
                    if (slot.AttackingCard != null || slot.DefendingCard != null)
                        return false;
                }

                return true;
            }


            public bool TrySetAttackingCard(Card card, int place)
            {
                var slot = slots[place];

                if (!slot.IsFree)
                {
                    return false;
                }

                slot.SetAttackingCard(card);
                return true;
            }


            public bool TrySetDefendingCard(Card card, int place)
            {
                var slot = slots[place];

                if (slot.AttackingCard == null)
                {
                    return false;
                }

                if (judge.GetStronger(card, slot.AttackingCard) == card) //может покрыть
                {
                    slot.SetDefendingCard(card);
                    return true;
                }

                return false;
            }


            public class Slot
            {
                public Card? AttackingCard { get; private set; }


                public Card? DefendingCard { get; private set; }


                public bool IsFree => AttackingCard == null && DefendingCard == null;


                public void SetAttackingCard(Card card)
                {
                    if (AttackingCard != null)
                    {
                        throw new InvalidOperationException();
                    }

                    AttackingCard = card;
                }


                public void SetDefendingCard(Card card)
                {
                    if (AttackingCard == null || DefendingCard != null)
                    {
                        throw new InvalidOperationException();
                    }

                    DefendingCard = card;
                }


                public void Free()
                {
                    AttackingCard = null;
                    DefendingCard = null;
                }
            }
        }


        public class Deck
        {
            private readonly List<Card> cards;
            private Random random;


            public Card? PopRandomCardOrNull()
            {
                if (cards.Count > 0)
                {
                    var index = random.Next(0, cards.Count);
                    var card = cards[index];
                    cards.RemoveAt(index);
                    return card;
                }

                return null;
            }


            public Deck()
            {
                cards = new List<Card>();
                random = new Random(DateTime.Now.Millisecond);

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        cards.Add(new Card((Rank) j, (Suit) i));
                    }
                }
            }
        }


        public class Game : IDisposable
        {
            protected List<AbstractPlayer> players;
            protected Round currentRound;
            protected Table table;
            protected Deck deck;
            protected Judge judge;
            private Random random = Random.Shared;


            public Card Trump { get; private set; }


            public bool IsPlaying { get; private set; }


            public Game(AbstractPlayer[] players)
            {
                if (players.Length < 2)
                    throw new ArgumentException();

                this.players = new List<AbstractPlayer>(players);
            }


            public void Start()
            {
                if (IsPlaying)
                {
                    throw new InvalidOperationException();
                }

                IsPlaying = true;

                ///Deck
                deck = new Deck();
                Trump = deck.PopRandomCardOrNull()!;

                judge = new Judge(Trump);

                table = new Table(judge);

                ///Inflating
                const int cardCountForPlayer = 6;

                foreach (var player in players)
                {
                    player.WantsPass += PlayerOnWantsPass;
                    player.AcceptDefeat += PlayerOnAcceptDefeat;
                    player.WantsThrowCard += PlayerOnWantsThrowCard;
                    
                    for (int i = 0; i < cardCountForPlayer; i++)
                    {
                        player.AddCard(deck.PopRandomCardOrNull()!);
                    }
                }

                //раздать карты
                //и опредить первого нападающего

                var list = new List<AbstractPlayer>(players);

                AbstractPlayer GetRandom() => list[random.Next(0, list.Count)];

                var attacker = GetRandom();
                list.Remove(attacker);
                var defender = GetRandom();

                TurnNextRound(new Round(attacker, defender));
            }


            [Pure]
            public AbstractPlayer NextFor(in AbstractPlayer player)
            {
                if (players.Count < 2)
                {
                    throw new InvalidOperationException();
                }

                return players[(players.IndexOf(player) + 1) % players.Count];
            }


            public void Dispose()
            {
                foreach (var player in players)
                {
                    player.WantsPass -= PlayerOnWantsPass;
                    player.AcceptDefeat -= PlayerOnAcceptDefeat;
                    player.WantsThrowCard -= PlayerOnWantsThrowCard;
                }
            }


            ~Game()
            {
                Dispose();
            }


            protected void PlayerOnWantsThrowCard(AbstractPlayer player, Card card, int slotIndex)
            {
                Debug($"{player.Name} tries throw card {card} on place {slotIndex}");

                if (player == currentRound.Attacker)
                {
                    if (table.TrySetAttackingCard(card, slotIndex))
                    {
                        player.RemoveCard(card);
                        NotifyAllPlayerThrowsCard(player, card, slotIndex);
                    }
                    else
                    {
                        Debug("Throw failed");
                    }
                }
                else if (player == currentRound.Defender)
                {
                    if (table.TrySetDefendingCard(card, slotIndex))
                    {
                        player.RemoveCard(card);
                        NotifyAllPlayerThrowsCard(player, card, slotIndex);
                    }
                    else
                    {
                        Debug("Throw failed");
                    }
                }
                else
                {
                    throw new Exception();
                }
            }


            protected void NotifyAllPlayerThrowsCard(AbstractPlayer thrower, Card card, int place)
            {
                Debug("Success throw");
                foreach (var player in players)
                {
                    player.NotifyPlayerThrowsCard(thrower, card, place);
                }
            }


            public void Debug(string content)
            {
                Console.WriteLine($"{Tag}{content}");
            }


            public string Tag => "Debug: ";


            protected void PlayerOnAcceptDefeat(AbstractPlayer player)
            {
                //TODO
            }


            protected void PlayerOnWantsPass(AbstractPlayer player)
            {
                if (currentRound.Attacker == player)
                {
                    if (table.IsAllAttackingCardsCovered() && !table.IsAllSlotsFree())
                    {
                        FinishRound(true);
                    }
                }

                if (currentRound.Defender == player)
                {
                    if (!table.IsAllAttackingCardsCovered() && !table.IsAllSlotsFree())
                    {
                        FinishRound(false);
                    }
                }
            }


            private void FinishRound(bool hasDefenderSuccess)
            {
                AbstractPlayer attacker, defender;

                if (hasDefenderSuccess)
                {
                    attacker = currentRound.Defender;
                    defender = currentRound.Attacker;
                }
                else
                {
                    attacker = currentRound.Attacker;
                    defender = currentRound.Defender;
                }

                TurnNextRound(new Round(attacker, defender));
            }


            protected virtual void TurnNextRound(Round round)
            {
                currentRound = round;
                foreach (var player in players)
                {
                    player.NotifyNextRound(currentRound);
                }
            }
        }


        public class Round
        {
            public AbstractPlayer Attacker { get; private set; }


            public AbstractPlayer Defender { get; private set; }


            public Round(AbstractPlayer attacker, AbstractPlayer defender)
            {
                if (attacker == null)
                    throw new ArgumentNullException(nameof(attacker));

                if (defender == null)
                    throw new ArgumentNullException(nameof(defender));

                Attacker = attacker;
                Defender = defender;
            }
        }


        public record GameEndResult(IEnumerable<AbstractPlayer> Winners);
    }
}