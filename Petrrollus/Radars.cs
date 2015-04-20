using Robocode;
using Robocode.Util;
using Petrrollus.EnemyInfoStorage;

namespace Petrrollus.Radars
{
    public interface IRadarProvider
    {
        void Init(Petrrollus robot, EnemyInfo info);
        void FirstTurn();
        void EveryTurn();
        void OnScan(ScannedRobotEvent evnt);
    }

    public class NarrowLockRadar : IRadarProvider
    {
        const int lostLockTime = 4;
        const int initTime = 35;

        private Petrrollus robot;
        private EnemyInfo info;

        public void Init(Petrrollus robot, EnemyInfo info)
        {
            this.robot = robot;
            this.info = info;
        }

        public void FirstTurn()
        {
            robot.IsAdjustRadarForGunTurn = true;
            robot.IsAdjustRadarForRobotTurn = true;

            robot.SetTurnRadarRightRadians(double.PositiveInfinity);
        }

        public void EveryTurn()
        {
            if (robot.Time < initTime)
            {
                robot.SetTurnRadarRightRadians(double.PositiveInfinity);
                return;
            }

            if (isLockLost())
            {
                double radarnTurn = (info.Bearings.GetCurrent() - robot.RadarHeadingDiffRadians >= 0) ? double.PositiveInfinity : double.NegativeInfinity;
                robot.SetTurnRadarRightRadians(radarnTurn);
            }

            robot.Scan();
        }

        private bool isLockLost()
        {
            return (robot.Time - info.Time.GetLast()) > lostLockTime;
        }

        public void OnScan(ScannedRobotEvent evnt)
        {
            double radarTurn = evnt.BearingRadians - robot.RadarHeadingDiffRadians;
            robot.SetTurnRadarRightRadians(Utils.NormalRelativeAngle(radarTurn * 1.2));
        }

    }
}
