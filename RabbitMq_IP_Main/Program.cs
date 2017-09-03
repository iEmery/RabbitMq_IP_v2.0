using RabbitMq_IP_Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq_IP_Main
{
    //  RabbitMq_IP_v2._0 is a program that use rabbitmq messaging solution 
    //  to integrate communication between 3 systems.
    //      *Hotel management system (HMS): Prestashop
    //      *Customer management system (CMS): suitCRM
    //      *Invoice management system (IMS): ninjaInvoice
    //
    //  Integration is only made by API's to send and receive the messages,
    //  so NO DATABASE URL is used by this program. 
    //  By running this program, endless loops will run to send en receive messages.
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("RabbitMq_IP_v2.0 strarted...");

            /// <summary>
            ///     RABBITMQ CONFIG
            /// </summary>
            /// 


            //API's config
            //  HMS api is used by adding a extern library (Prestasharp client)
            //  https://github.com/Bukimedia/PrestaSharp
            //  HMS api use no password, and must generate account first in admin panel-webservices.
            //  Access to this account has permisson to all tables.
            string HMS_api_URL = "http://localhost/hotelcommerce-1.1.0/api";
            string HMS_api_ACCOUNT = "TDRWA14TSJ7PW357BXVK6MV1I675FIJM";
            string HMS_api_PASSWORD = "";

            //  CMS api is used by adding a Service Reference to this program.
            //  CMS api use the SOAP protocol to access the api.
            //  SoapUI was used to explore the api.
            string CMS_api_URL = "http://localhost/SuiteCRM-7.9.4/service/v4_1/soap.php/?wsdl";
            string CMS_api_ACCOUNT = "admin";
            string CMS_api_PASSWORD = "21232f297a57a5a743894a0e4a801fc3";

            //  IMS api used by a simple HTTP vlient request.
            //  IMS api uses a api token that must be generated first in thet
            string IMS_api_URL = "http://localhost/ninja/public/api/v1/";
            string IMS_api_ACCOUNT = "ivj8ll3diiotu79rc3zxkdmmbu8oblmr";
            string IMS_api_PASSWORD = "";
            
            
            
            
            /// <summary>
            ///     RABBITMQ SENDERS AND RECEIVERS
            /// </summary>
            /// 


            //a2756b7c-013c-49e9-a4a2-80100387e2f6
            //NEW HMS CUSTOMER SENDER
            Task hms_crm_sender = new Task(
                RabbitMq_IP.Notify_new_hms_customers(
                    HMS_api_URL, HMS_api_ACCOUNT, HMS_api_PASSWORD, "HMS"));


            //a2756b7c-013c-49e9-a4a2-80100387e2f6
            //NEW HMS CUSTOMER RECEIVER
            Task hms_crm_receiver = new Task(
                RabbitMq_IP.New_hms_customer_notified(
                    CMS_api_URL, CMS_api_ACCOUNT, CMS_api_PASSWORD, "CRM"));

            //0a9c1bf4-2436-4e3e-bbdf-0bb0d2162bf7
            //HMS send to NINJA nieuwe factuur data
            Task hms_ninja_sender = new Task(
                RabbitMq_IP.Notify_new_orders(
                    HMS_api_URL, HMS_api_ACCOUNT, HMS_api_PASSWORD, "HMS"));

            

            //0a9c1bf4-2436-4e3e-bbdf-0bb0d2162bf7
            //NEW HMS ORDER RECEIVER
            Task hms_ninja_receiver = new Task(
                RabbitMq_IP.New_hms_order_notified(
                    HMS_api_URL, HMS_api_ACCOUNT, HMS_api_PASSWORD, "IMS"));


            //6070698c-ff4b-417b-bce8-b18adc4be7c4
            //CRM send nieuwe klanten (confirmed) data
            Task crm_sender = new Task(
                RabbitMq_IP.Notify_new_crm_customers(
                    CMS_api_URL, CMS_api_ACCOUNT, CMS_api_PASSWORD, "CRM"));


            //73444267-e6ee-42ad-b2dd-db8d76e73a06
            //NEW CRM CUSTOMER TO IMS RECEIVER
            Task crm_ninja_receiver = new Task(
                RabbitMq_IP.New_crm_ims_customer_notified(
                    CMS_api_URL, CMS_api_ACCOUNT, CMS_api_PASSWORD, "IMS"));
            



            //1237bc42-eac0-4add-9dc1-075d86a590ac
            //NEW POS CUSTOMER RECEIVER
            Task crm_hms_receiver = new Task(
                RabbitMq_IP.New_crm_customer_notified(
                    HMS_api_URL, HMS_api_ACCOUNT, HMS_api_PASSWORD, "HMS"));


            /// <summary>
            ///     RABBITMQ RUN
            /// </summary
            /// 


            Console.WriteLine("Senders started...");
            //RABBITMQ SENDERS
            //hms_crm_sender.Start();
            //hms_ninja_sender.Start();
            //crm_sender.Start();


            Console.WriteLine("Receivers started...\n");
            //RABBITMQ RECEIVERS
            //hms_crm_receiver.Start();
            //hms_crm_receiver.Wait();
            //crm_hms_receiver.Start();
            //crm_hms_receiver.Wait();
            Console.ReadKey();
        }
    }
}
