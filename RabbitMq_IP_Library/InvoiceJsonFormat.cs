using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq_IP_Library
{
    public class InvoiceJsonFormat
    {
        public string account_key { get; set; }
        public bool is_owner { get; set; }
        public int id { get; set; }
        public int amount { get; set; }
        public int balance { get; set; }
        public int client_id { get; set; }
        public int invoice_status_id { get; set; }
        public int updated_at { get; set; }
        public object archived_at { get; set; }
        public string invoice_number { get; set; }
        public int discount { get; set; }
        public string po_number { get; set; }
        public string invoice_date { get; set; }
        public string due_date { get; set; }
        public string terms { get; set; }
        public string public_notes { get; set; }
        public string private_notes { get; set; }
        public bool is_deleted { get; set; }
        public int invoice_type_id { get; set; }
        public bool is_recurring { get; set; }
        public int frequency_id { get; set; }
        public object start_date { get; set; }
        public object end_date { get; set; }
        public object last_sent_date { get; set; }
        public int recurring_invoice_id { get; set; }
        public string tax_name1 { get; set; }
        public int tax_rate1 { get; set; }
        public string tax_name2 { get; set; }
        public int tax_rate2 { get; set; }
        public bool is_amount_discount { get; set; }
        public string invoice_footer { get; set; }
        public int partial { get; set; }
        public bool has_tasks { get; set; }
        public bool auto_bill { get; set; }
        public int custom_value1 { get; set; }
        public int custom_value2 { get; set; }
        public bool custom_taxes1 { get; set; }
        public bool custom_taxes2 { get; set; }
        public bool has_expenses { get; set; }
        public int quote_invoice_id { get; set; }
        public object custom_text_value1 { get; set; }
        public object custom_text_value2 { get; set; }
        public bool is_quote { get; set; }
        public bool is_public { get; set; }
        public Invoice_Items[] invoice_items { get; set; }
    }

    public class Invoice_Items
    {
        public string account_key { get; set; }
        public bool is_owner { get; set; }
        public int id { get; set; }
        public string product_key { get; set; }
        public int updated_at { get; set; }
        public object archived_at { get; set; }
        public string notes { get; set; }
        public int cost { get; set; }
        public int qty { get; set; }
        public string tax_name1 { get; set; }
        public int tax_rate1 { get; set; }
        public string tax_name2 { get; set; }
        public int tax_rate2 { get; set; }
        public int invoice_item_type_id { get; set; }
    }

}
