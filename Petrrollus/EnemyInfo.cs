using Robocode;
using Robocode.Util;
using Petrrollus.Helpers;
using System;

namespace Petrrollus.EnemyInfoStorage
{
    public class EnemyInfo
    {
        const int HistoryLenght = 40;
        const double GuessCoeficient = 0.5;

        public CyclicDoubleArray Bearings { get; private set; }
        public CyclicDoubleArray Distance { get; private set; }
        public CyclicDoubleArray Energy { get; private set; }
        public CyclicDoubleArray Heading { get; private set; }
        public CyclicDoubleArray Velocity { get; private set; }
        public CyclicDoubleArray Time { get; private set; }

        public CyclicDoubleArray OriginAngle { get; private set; }

        public CyclicCoordinatesArray Position { get; private set; }

        public EnemyInfo()
        {
            Bearings = new CyclicDoubleArray(HistoryLenght);
            Distance = new CyclicDoubleArray(HistoryLenght);
            Energy = new CyclicDoubleArray(HistoryLenght);
            Heading = new CyclicDoubleArray(HistoryLenght);
            Velocity = new CyclicDoubleArray(HistoryLenght);
            Time = new CyclicDoubleArray(HistoryLenght);

            OriginAngle = new CyclicDoubleArray(HistoryLenght);
            Position = new CyclicCoordinatesArray(HistoryLenght);

        }

        public Coordinates GetGuessPositionHistory(int ticks)
        {
            Coordinates inc = ((Position.GetDifference(1) + (Position.GetDifference(Position.LastIndex) / Position.LastIndex) + (Position.GetDifference(Position.LastIndex / 2) / (Position.LastIndex / 2))) / 3) * ticks;
            return inc + Position.GetCurrent();
        }

        public Coordinates GetGuessPositionHeading(int ticks)
        {
            double lenght = ticks * Velocity.GetCurrent();
            double angle = Heading.GetCurrent();

            double incX = lenght * Math.Sin(angle);
            double incY = lenght * Math.Cos(angle);

            return new Coordinates(incX, incY) + Position.GetCurrent();
        }

        public Coordinates GetGuessPositionEducated(int ticks)
        {
            Coordinates headingGuess = GetGuessPositionHeading(ticks);
            Coordinates historyGuess = GetGuessPositionHistory(ticks);

            return headingGuess + (headingGuess - historyGuess) * GuessCoeficient;
        }

        public void UpdateInfo(ScannedRobotEvent evnt, Coordinates robotPostion, double robotHeadings)
        {
            Bearings.AddNew(evnt.BearingRadians);
            Distance.AddNew(evnt.Distance);
            Energy.AddNew(evnt.Energy);
            Heading.AddNew(evnt.HeadingRadians);
            Velocity.AddNew(evnt.Velocity);
            Time.AddNew(evnt.Time);

            OriginAngle.AddNew(Utils.NormalRelativeAngle(robotHeadings + Bearings.GetCurrent()));

            double y = Distance.GetCurrent() * Math.Cos(OriginAngle.GetCurrent());
            double x = Distance.GetCurrent() * Math.Sin(OriginAngle.GetCurrent());

            Position.AddNew(robotPostion + new Coordinates(x, y));
        }

    }
}
