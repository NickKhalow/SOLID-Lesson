namespace SOLID.Princeples;

public class DIP
{
    public class DurakGame
    {
        public enum Rank
        {
            clubs = 0,
            diamonds = 1,
            hearts = 2,
            sprades = 3
        }

        public enum Suit
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
            public event Action<Card> ThrowCard;

            protected List<Card> _cards;

            public abstract void AddCard(Card card);

            public abstract void NotifyNextRound(Round round);

            public abstract void NotifyPlayerThrowsCard(AbstractPlayer player, Card card);

            //Хорошо
            public abstract void NotifyGameEnded(GameEndResult gameEndResult);

            //Плохо: если у нас добавятся ещё данные? Например сколько длилась игра, кто какие карты бросил и прочее
            //Метод будет разрастаться
            public abstract void NotifyGameEnded(IEnumerable<AbstractPlayer> winners);
        }

        public abstract class Table
        {
            protected AbstractPlayer[] _players;

            protected Round _currentRound;

            protected Table(AbstractPlayer[] players)
            {
                _players = players;
                TurnNextRound();
            }

            public void TurnNextRound()
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