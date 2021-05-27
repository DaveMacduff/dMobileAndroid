using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace dMobile.Resources.Models
{
    //string sqlTable = "CREATE TABLE IF NOT EXISTS tActivityPickingRoute"
    //+ "("
    //+ "PickingActivityRefno INTEGER NOT NULL,"
    //+ "RouteRefno INTEGER NOT NULL,"
    //+ "RouteStop INTEGER NOT NULL, "

    //+ "CONSTRAINT ActivityPickingRouteToActivityPickingOnPickingActivityRefnoAndActivityRefno FOREIGN KEY (PickingActivityRefno) REFERENCES tActivityPicking(ActivityRefno) ON DELETE CASCADE ON UPDATE CASCADE, "
    //+ "CONSTRAINT ActivityPickingRouteToRouteOnRouteRefno FOREIGN KEY (RouteRefno) REFERENCES tRoute(RouteRefno) ON DELETE CASCADE ON UPDATE CASCADE"

    //+ ")";

    //        if (this.OnNubStatOut != null)
    //        {
    //    // TODO: Resource strings
    //    this.MsgEventArgs.StatusOutMsg = "Creating picking route table.";
    //    this.OnNubStatOut(this, this.MsgEventArgs);
    //}

    //await this.RunDDLCommand(sqlTable);

    //        string sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS UQ_ActivityPickingRouteOnPickingActivityRefnoAndRouteRefno "
    //            + "ON tActivityPickingRoute(PickingActivityRefno ASC, RouteRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE UNIQUE INDEX IF NOT EXISTS FK_ActivityPickingRouteOnPickingActivityRefno ON "
    //            + "tActivityPickingRoute(PickingActivityRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    //sqlNdx = "CREATE INDEX IF NOT EXISTS FK_ActivityPickingRouteOnRouteRefno ON tActivityPickingRoute(RouteRefno ASC)";
    //await this.RunDDLCommand(sqlNdx);

    [Table("tActivityPickingRoute")]
    public class tActivityPickingRouteModel
    {
        [PrimaryKey, NotNull, ForeignKey(typeof(tActivityPickingModel)), Indexed(Name = "UQ_ActivityPickingRouteOnPickingActivityRefnoAndRouteRefno", Order = 1, Unique = true)]
        public int PickingActivityRefno {get; set;}

        [NotNull, ForeignKey(typeof(tRouteModel)), Indexed(Name = "UQ_ActivityPickingRouteOnPickingActivityRefnoAndRouteRefno", Order = 2, Unique = true)]
        public int RouteRefno { get; set; }

        [NotNull, Indexed(Name = "FK_ActivityPickingRouteOnRouteRefno", Order = 1, Unique = false)]
        public int RouteStop { get; set; }
    }
}
