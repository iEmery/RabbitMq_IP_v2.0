﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMq_IP_v2._0
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
            string CMS_api_ACCOUNT = "";
            string CMS_api_PASSWORD = "";

            //  IMS api used by a simple HTTP vlient request.
            //  IMS api uses a api token that must be generated first in thet
            string IMS_api_URL = "http://localhost/ninja/public/api/v1/";
            string IMS_api_ACCOUNT = "ivj8ll3diiotu79rc3zxkdmmbu8oblmr";
            string IMS_api_PASSWORD = "";


        }
    }
}
