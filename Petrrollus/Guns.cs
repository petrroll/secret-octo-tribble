using Petrrollus.EnemyInfoStorage;
using Petrrollus.Helpers;
using Robocode;
using Robocode.Util;
using System.Console;
using System;

namespace Petrrollus.Guns
{
    public interface IAimAndShootProvider
    {
        void Init(Petrrollus robot, EnemyInfo info);
        void FirstTurn();
        void EveryTurn();
    }

    public class PrecisionGun : IAimAndShootProvider
    {
        const double highMaxDist = 150;
        const double mediMaxDist = 275;
        const double lowMaxDist = 600;

        const double maxGunTurn = 0.34; //20 deg.

        Petrrollus robot;
        EnemyInfo info;

        public void Init(Petrrollus robot, EnemyInfo info)
        {
            this.robot = robot;
            this.info = info;
        }

        public void FirstTurn()
        {
            robot.IsAdjustGunForRobotTurn = true;
        }

        public void EveryTurn()
        {
            Coordinates mePosition = robot.Position;


            double enemyCurrDistance = (info.Position.GetCurrent() - mePosition).Distance();
            double bulletPower = determineBulletPower(enemyCurrDistance);
            double travelTime = getBulletTravelTime(bulletPower, enemyCurrDistance);

            Coordinates enemyFutPosition = info.GetGuessPositionEducated((int)travelTime);
            double gunTurn = getGunTurn(enemyFutPosition);

            robot.SetTurnGunRightRadians(gunTurn);
            if (gunTurn < maxGunTurn) { robot.SetFireBullet(bulletPower); }
        }

        private double getGunTurn(Coordinates enemyFutPosition)
        {
            double enemyFutAngle = MiscHelper.GetAngleBetweenPositions(robot.Position, enemyFutPosition);
            double gunTurn = Utils.NormalRelativeAngle(enemyFutAngle - robot.GunHeadingRadians);
            return gunTurn;
        }

        private static double getBulletTravelTime(double bulletPower, double enemyCurrDistance)
        {
            double bulletSpeed = Rules.GetBulletSpeed(bulletPower);
            double travelTime = enemyCurrDistance / bulletSpeed;
            return travelTime;
        }

        private static double determineBulletPower(double enemyCurrDistance)
        {
            double bulletPower;

            if (enemyCurrDistance < highMaxDist) { bulletPower = 3; }
            else if (enemyCurrDistance < mediMaxDist) { bulletPower = 2; }
            else if (enemyCurrDistance < lowMaxDist) { bulletPower = 1; }
            else { bulletPower = 0; }

            return bulletPower;
        }
    }
}
