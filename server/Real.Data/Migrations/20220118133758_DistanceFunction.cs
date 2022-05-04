using Microsoft.EntityFrameworkCore.Migrations;

namespace Real.Data.Migrations
{
    public partial class DistanceFunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                drop function if exists DistanceBetweenPoints;

                DELIMITER $$
                CREATE DEFINER=`admin`@`%` FUNCTION `DistanceBetweenPoints`(lat1 double, lon1 double, lat2 double, lon2 double) RETURNS double
                    DETERMINISTIC
                BEGIN
                    -- RETURN ST_Distance_Sphere(point(lat1, lon1), point(lat2, lon2));
                    --     return 111.111 *
                    -- 		DEGREES(ACOS(LEAST(1.0, COS(RADIANS(lat1))
                    -- 			 * COS(RADIANS(lon2))
                    -- 			 * COS(RADIANS(lon1 - lon2))
                    -- 			 + SIN(RADIANS(lat1))
                    -- 			 * SIN(RADIANS(lat2)))));

                    -- https://www.movable-type.co.uk/scripts/latlong.html
                    set @R = 6371e3; -- metres
                    set @PI = 3.141592653589793;
                    set @phi1 = lat1 * @PI / 180.0;
                    set @phi2 = lat2 * @PI / 180.0;
                    set @dPhi = (lat2-lat1) * @PI / 180.0;
                    set @dLambda = (lon2-lon1) * @PI / 180.0;

                    set @a =  sin(@dPhi/2) * sin(@dPhi/2) +
                            cos(@phi1) * cos(@phi2) *
                            sin(@dLambda / 2.0) * sin(@dLambda / 2.0);
                    set @c = 2.0 * atan2(sqrt(@a), sqrt(1.0 - @a));

                    set @d = @R * @c; -- in metres
                    
                    return @d;

                END$$
                DELIMITER ;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS DistanceBetweenPoints;");
        }
    }
}
