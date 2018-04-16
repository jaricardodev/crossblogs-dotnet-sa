using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace crossblog.Migrations
{
    public partial class AdSprocForSearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Uncomment to test search using a sproc
            //migrationBuilder.Sql(@"DELIMITER $$
            //                        CREATE  PROCEDURE searchSproc(
            //                        textsearch varchar(65535)
            //                        )
            //                        BEGIN
            //                                            SELECT `articles`.`Id`,
            //                                            `articles`.`Content`,
            //                                            `articles`.`Date`,
            //                                            `articles`.`Published`,
            //                                            `articles`.`Title`,
            //                                            `articles`.`Created_At`,
            //                                            `articles`.`Updated_At`
            //                                            FROM `crossblog`.`articles` 
            //                                            where (articles.Title like concat('%',textsearch,'%') OR articles.Content like concat('%',textsearch,'%'));
            //                                END$$
            //                        DELIMITER ;
            //                        ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //Uncomment to test search using a sproc
            //migrationBuilder.Sql("DROP PROCEDURE `crossblog`.`searchSproc`;");
        }
    }
}
