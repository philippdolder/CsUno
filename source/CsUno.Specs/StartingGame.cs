namespace CsUno
{
    using System;
    using FluentAssertions;
    using Xbehave;
    using Xunit;

    public class StartingGame
    {
        [Scenario]
        public void WithTwoPlayers(Host host, Game game)
        {
            "Given a Host"
                ._(() => host = new Host());

            const int MinimumNumberOfPlayers = 2;
            "When starting a game with 2 players"
                ._(() => game = host.StartGame(MinimumNumberOfPlayers));

            "Then a GameStarted event is emitted"
                ._(() => game.);
        }

        [Scenario]
        public void WithTenPlayers(Host host, Game game)
        {
            "Given a Host"
                ._(() => host = new Host());

            const int MaximumNumberOfPlayers = 10;
            "When starting a game with 10 players"
                ._(() => game = host.StartGame(MaximumNumberOfPlayers));

            "Then the current player is Player 1"
                ._(() => game.CurrentPlayer.Should().Be(1));
        }

        [Scenario]
        public void WithOnePlayer(Host host, Exception exception)
        {
            "Given a Host"
                ._(() => host = new Host());

            "When starting a game with 1 player"
                ._(() => exception = Record.Exception(() => host.StartGame(1)));

            "Then an ArgumentOutOfRangeException is thrown"
                ._(() => exception.Should().BeOfType<ArgumentOutOfRangeException>());
        }

        [Scenario]
        public void WithElevenPlayers(Host host, Exception exception)
        {
            "Given a Host"
                ._(() => host = new Host());

            "When starting a game with 11 players"
                ._(() => exception = Record.Exception(() => host.StartGame(11)));

            "Then an ArgumentOutOfRangeException is thrown"
                ._(() => exception.Should().BeOfType<ArgumentOutOfRangeException>());
        }
    }

    public class Game
    {
        private readonly int numberOfPlayers;

        public Game(int numberOfPlayers)
        {
            this.numberOfPlayers = numberOfPlayers;

            this.CurrentPlayer = 1;
        }

        public int CurrentPlayer { get; private set; }
    }

    public class StartGame : Command
    {
        public StartGame(int numberOfPlayers, GameId id)
        {
            this.NumberOfPlayers = numberOfPlayers;
            this.Id = id;
        }

        public int NumberOfPlayers { get; }

        public GameId Id { get; }
    }

    public class GameId : IEquatable<GameId>
    {
        private readonly Guid id;

        private GameId(Guid id)
        {
            this.id = id;
        }

        public static GameId New()
        {
            return new GameId(Guid.NewGuid());
        }

        public static GameId Create(Guid id)
        {
            return new GameId(id);
        }

        public bool Equals(GameId other)
        {
            return this.id.Equals(other.id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((GameId)obj);
        }

        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
    }

    public abstract class Command : Message
    {
    }

    public abstract class Event : Message
    {
    }

    public abstract class Message
    {
    }

    public class Host
    {
        private const int MinimumNumberOfPlayers = 2;
        private const int MaximumNumberOfPlayers = 10;

        public Game StartGame(int numberOfPlayers)
        {
            if (NotEnoughPlayers(numberOfPlayers) || TooManyPlayers(numberOfPlayers))
            {
                throw new ArgumentOutOfRangeException(nameof(numberOfPlayers), 1, $"must be between {MinimumNumberOfPlayers} and {MaximumNumberOfPlayers}.");
            }

            return new Game(numberOfPlayers);
        }

        private static bool TooManyPlayers(int numberOfPlayers)
        {
            return numberOfPlayers > MaximumNumberOfPlayers;
        }

        private static bool NotEnoughPlayers(int numberOfPlayers)
        {
            return numberOfPlayers < MinimumNumberOfPlayers;
        }
    }
}