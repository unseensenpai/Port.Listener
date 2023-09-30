using DevExpress.Xpo;

namespace PortListener.DAL
{
    [Persistent("public.tbl_ipchart")]
    public class PortsDAL : XPLiteObject
    {
        public PortsDAL(Session session) : base(session) { }
        [Persistent("id"), Key] public int Id { get; set; }
        [Persistent("ipaddress")] public string IpAddress { get; set; }
        [Persistent("branch_id")] public int BranchId { get; set; }
        [Persistent("error_count")] public int ErrorCount { get; set; }
        [Persistent("total_errors")] public int TotalErrors { get; set; }
        [Persistent("last_trigger_time")] public DateTime LastTriggerTime { get; set; }
        [Persistent("is_active")] public bool IsActive { get; set; }
    }
}
