using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq_IP_Library
{
    public class ClientsJsonFormat
    {
        public string account_key { get; set; }
        public bool is_owner { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int balance { get; set; }
        public int paid_to_date { get; set; }
        public int updated_at { get; set; }
        public object archived_at { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postal_code { get; set; }
        public int country_id { get; set; }
        public string work_phone { get; set; }
        public string private_notes { get; set; }
        public object public_notes { get; set; }
        public object last_login { get; set; }
        public string website { get; set; }
        public int industry_id { get; set; }
        public int size_id { get; set; }
        public bool is_deleted { get; set; }
        public int payment_terms { get; set; }
        public string vat_number { get; set; }
        public string id_number { get; set; }
        public int language_id { get; set; }
        public int currency_id { get; set; }
        public object custom_value1 { get; set; }
        public object custom_value2 { get; set; }
        public int invoice_number_counter { get; set; }
        public int quote_number_counter { get; set; }
        public Contact[] contacts { get; set; }
    }

    public class Contact
    {
        public string account_key { get; set; }
        public bool is_owner { get; set; }
        public int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public int updated_at { get; set; }
        public object archived_at { get; set; }
        public bool is_primary { get; set; }
        public string phone { get; set; }
        public object last_login { get; set; }
        public bool send_invoice { get; set; }
        public object custom_value1 { get; set; }
        public object custom_value2 { get; set; }
    }

}
