using System.Diagnostics.Contracts;
using System.IO.Pipes;

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

        public record Card(Rank Rank, Suit Suit);

        public abstract class AbstractPlayer
        {
            public event Action Pass, AcceptDefeat;
            public event Action<Card, int> ThrowCard;

            protected List<Card> _cards;

            public abstract void AddCard(Card card);

            public abstract void NotifyNextRound(Round round);

            public abstract void NotifyPlayerThrowsCard(AbstractPlayer player, Card card, int slotIndex);

            //Хорошо
            public abstract void NotifyGameEnded(GameEndResult gameEndResult);

            //Плохо: если у нас добавятся ещё данные? Например сколько длилась игра, кто какие карты бросил и прочее
            //Метод будет разрастаться
            public abstract void NotifyGameEnded(IEnumerable<AbstractPlayer> winners);
        }

        public abstract class Table : IDisposable
        {
            protected List<AbstractPlayer> _players;

            protected Round _currentRound;

            protected Table(AbstractPlayer[] players)
            {
                _players = new List<AbstractPlayer>(players);

                foreach (var player in players)
                {
                    player.Pass += PlayerOnPass;
                    player.AcceptDefeat += PlayerOnAcceptDefeat;
                    player.ThrowCard += PlayerOnThrowCard;
                }


                TurnNextRound();
            }

            [Pure]
            public AbstractPlayer NextFor(in AbstractPlayer player)
            {
                if (_players.Count < 2)
                {
                    throw new InvalidOperationException();
                }

                return _players[(_players.IndexOf(player) + 1) % _players.Count];
            }

            public void Dispose()
            {
                foreach (var player in _players)
                {
                    player.Pass -= PlayerOnPass;
                    player.AcceptDefeat -= PlayerOnAcceptDefeat;
                    player.ThrowCard -= PlayerOnThrowCard;
                }
            }

            ~Table()
            {
                Dispose();
            }

            protected abstract void PlayerOnThrowCard(Card card, int slotIndex);

            protected abstract void PlayerOnAcceptDefeat();

            protected abstract void PlayerOnPass();

            public virtual void TurnNextRound()
            {
                //_currentRound = new Round(); //TODO
                foreach (var abstractPlayer in _players)
                {
                    abstractPlayer.NotifyNextRound(_currentRound);
                }
            }
        }

        public class Round
        {
            public AbstractPlayer Attacker { get; private set; }
            public AbstractPlayer Defender { get; private set; }

            protected Round(AbstractPlayer attacker, AbstractPlayer defender)
            {
                Attacker = attacker;
                Defender = defender;
            }
        }

        public record GameEndResult(IEnumerable<AbstractPlayer> Winners);
    }
}