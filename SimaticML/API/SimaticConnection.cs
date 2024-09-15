namespace SimaticML.API
{
    public abstract class SimaticConnection(SimaticConnection? previousConnection = null)
    {
        public SimaticConnection? PreviousConnection { get; init; } = previousConnection;
        public SimaticConnection? NextConnection { get; set; }

        public abstract SimaticConnection AND(object part);

        public abstract SimaticConnection OR(object part);

        public static SimaticConnection operator &(SimaticConnection connection, object next) => connection.AND(next);
        public static SimaticConnection operator |(SimaticConnection connection, object next) => connection.OR(next);
    }

    public class SimaticSelfConnection(SimaticPart part) : SimaticConnection()
    {
        public SimaticPart Part { get; init; } = part;

        public override SimaticConnection AND(object next)
        {
            SimaticConnection? connection = null;
            if (next is SimaticPart nextPart)
            {
                connection = new SimaticANDConnection(this) { Connection = new SimaticSelfConnection(nextPart) };
            }
            else if (next is SimaticConnection nextConnection)
            {
                connection = new SimaticANDConnection(this) { Connection = nextConnection };
            }

            ArgumentNullException.ThrowIfNull(connection, nameof(connection));

            this.NextConnection = connection;
            return connection;
        }

        public override SimaticConnection OR(object next)
        {
            var orConnection = new SimaticORConnection(this);
            orConnection.PartList.Add(this.Part);
            if (next is SimaticPart nextPart)
            {
                orConnection.PartList.Add(nextPart);
            }
            else if (next is SimaticSelfConnection nextSelfConnection)
            {
                orConnection.PartList.Add(nextSelfConnection.Part);
            }

            return orConnection;
        }
    }

    public class SimaticANDConnection(SimaticConnection? previousConnection = null) : SimaticConnection(previousConnection)
    {
        public SimaticConnection? Connection { get; set; }

        public override SimaticConnection AND(object next)
        {
            SimaticConnection? connection = null;
            if (next is SimaticPart nextPart)
            {
                connection = new SimaticANDConnection(this) { Connection = new SimaticSelfConnection(nextPart) };
            }
            else if (next is SimaticConnection nextConnection)
            {
                connection = new SimaticANDConnection(this) { Connection = nextConnection };
            }

            ArgumentNullException.ThrowIfNull(connection, nameof(connection));

            this.NextConnection = connection;
            return connection;
        }

        public override SimaticConnection OR(object part)
        {
            var orConnection = new SimaticORConnection(this);
            this.NextConnection = orConnection;
            return this;
        }
    }

    public class SimaticORConnection(SimaticConnection? previousConnection = null) : SimaticConnection(previousConnection)
    {
        public List<SimaticPart> PartList { get; init; } = [];

        public override SimaticConnection AND(object next)
        {
            SimaticConnection? connection = null;
            if (next is SimaticPart nextPart)
            {
                connection = new SimaticANDConnection(this) { Connection = new SimaticSelfConnection(nextPart) };
            }
            else if (next is SimaticConnection nextConnection)
            {
                connection = new SimaticANDConnection(this) { Connection = nextConnection };
            }

            ArgumentNullException.ThrowIfNull(connection, nameof(connection));

            this.NextConnection = connection;
            return connection;
        }

        public override SimaticConnection OR(object next)
        {
            if (next is SimaticPart nextPart)
            {
                this.PartList.Add(nextPart);
            }
            else if (next is SimaticSelfConnection nextSelfConnection)
            {
                this.PartList.Add(nextSelfConnection.Part);
            }
            else if (next is SimaticORConnection nextORConnection)
            { //Making an or with another or collection will just incorporate it.
                this.PartList.AddRange(nextORConnection.PartList);
            }
            return this;
        }
    }
}
