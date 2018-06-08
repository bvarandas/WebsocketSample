using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gradual.Spider.SupervisorRisco.Lib.Dados
{
    public class ErrorMessages
    {
        // Error codes
        public static int ERROR                                 = -1;
        public static int OK                                    = 0;
        public static int ERR_CODE_PARAM_PERM_NOT_FOUND         = 2000;
        public static int ERR_CODE_MAX_PATRIMONIAL_ACHIEVED     = 2001;
        public static int ERR_CODE_MAX_LOSS_ACHIEVED            = 2002;
        public static int ERR_CODE_DATA_NOT_LOADED              = 2003;
        public static int ERR_CODE_SYMBOL_NOT_FOUND             = 2004;
        public static int ERR_CODE_NO_PERM_VISTA                = 2005;
        public static int ERR_CODE_NO_PERM_OPCAO                = 2006;
        public static int ERR_CODE_NO_PERM_FUT                  = 2007;
        public static int ERR_CODE_INSTRUMENT_GLOBAL_BLOCKED    = 2008;
        public static int ERR_CODE_INSTRUMENT_GROUP_BLOCKED     = 2009;
        public static int ERR_CODE_INSTRUMENT_CLIENT_BLOCKED    = 2010;
        public static int ERR_CODE_FAT_FINGER_NOT_FOUND         = 2011;
        public static int ERR_CODE_ORDER_LIMIT_EXCEEDED         = 2012;
        public static int ERR_CODE_SERIES_OPTION_BLOCKED        = 2013;
        public static int ERR_CODE_OPERATING_LIMIT_NOT_FOUND    = 2014;
        public static int ERR_CODE_OPERATING_LIMIT_EXCEEDS      = 2015;
        public static int ERR_CODE_BMF_LIMIT_NOT_FOUND          = 2016;
        public static int ERR_CODE_BMF_LIMIT_CONTRACT_EXCEEDS   = 2017;
        public static int ERR_CODE_BMF_LIMIT_INSTRUMENT_NOT_FOUND = 2018;
        public static int ERR_CODE_BMF_LIMIT_QTD_EXCEEDS        = 2019;
        public static int ERR_CODE_BMF_LIMIT_OPERATING_EXCEEDS_CONTRACT  = 2020;
        public static int ERR_CODE_BMF_LIMIT_OPERATING_EXCEEDS_INST = 2021;
        public static int ERR_CODE_TO_ORDER_NOT_FOUND           = 2022;
        public static int ERR_CODE_FAT_FINGER_BASE_PRICE_ZEROED = 2023;
        public static int ERR_CODE_OMS_SENDING_ORDER            = 2024;
        public static int ERR_CODE_TEST_INSTRUMENT_NOT_FOUND = 2025;
        public static int ERR_CODE_INSTITUTIONAL_PROFILE_FOUND = 2026;
        public static int ERR_CODE_GIVE_UP_ACCOUNT = 2027;
        public static int ERR_CODE_SPIDER_PERMISSION = 2028;
        public static int ERR_CODE_LIMIT_FIX_NOT_FOUND = 2029;
        public static int ERR_CODE_LIMIT_FIX_FAT_FINGER = 2030;
        public static int ERR_CODE_LIMIT_FIX_EXCEEDS = 2031;
        public static int ERR_CODE_MAX_LOSS_EXCEEDED = 2032;

        // Messages
        public static string MSG_OK = "Ok";
        public static string ERR_PARAM_PERM_NOT_FOUND = "Parameter / permission not found for current client account";
        public static string ERR_MAX_PATRIMONIAL_ACHIEVED = "Maximum loss achieved. [{0}] of total patrimonial reached. Please, contact the risk department";
        public static string ERR_MAX_LOSS_ACHIEVED = "Maximum patrimonial loss achieved. [{0}] of loss. Please, contact the risk department";
        public static string ERR_DATA_NOT_LOADED = "Information not loaded";
        public static string ERR_SYMBOL_NOT_FOUND = "Instrument not found";
        public static string ERR_NO_PERM_VISTA = "Client not allowed to trade in equities market. Client account: {0}";
        public static string ERR_NO_PERM_OPCAO = "Client not allowed to trade in options market. Client account: {0}";
        public static string ERR_NO_PERM_FUT = "Client not allowed to trade in futures market. Client account: {0}";
        public static string ERR_INSTRUMENT_GLOBAL_BLOCKED = "Instrument blocked. Client not allowed to trade this instrument";
        public static string ERR_INSTRUMENT_GROUP_BLOCKED = "Client Group is not allowed to trade this instrument";
        public static string ERR_INSTRUMENT_CLIENT_BLOCKED = "Client is not allowed to trade this instrument";
        public static string ERR_FAT_FINGER_NOT_FOUND = "Fat finger permission not found";
        public static string ERR_ORDER_LIMIT_EXCEEDED = "The order sent exceed the maximum limit allowed per order";
        public static string ERR_SERIES_OPTION_BLOCKED = "Serie:[{0}] is blocked for trading";
        public static string ERR_OPERATING_LIMIT_NOT_FOUND = "Client operational limit exceeded to trade in market [{0}]";
        public static string ERR_OPERATING_LIMIT_EXCEEDS = "Client does not have sufficient operational limit to send this order";
        public static string ERR_BMF_LIMIT_NOT_FOUND = "Client does not have bmf limit to operate in future market";
        public static string ERR_BMF_LIMIT_CONTRACT_EXCEEDS = "Client does not have sufficient limit to trade this contract. Please, contact the risk department.";
        public static string ERR_BMF_LIMIT_INSTRUMENT_NOT_FOUND = "Client does not have sufficient limit to trade this instrument.";
        public static string ERR_BMF_LIMIT_QTD_EXCEEDS = "Order quantity exceeds the maximum limit allowed per order. Total allowed: {0}";
        public static string ERR_BMF_LIMIT_OPERATING_EXCEEDS_CONTRACT = "Insufficient operational limit to trade this contract";
        public static string ERR_BMF_LIMIT_OPERATING_EXCEEDS_INST = "Insufficient operational limit to trade this instrument";
        public static string ERR_TO_ORDER_NOT_FOUND = "Original order not found for replace";
        public static string ERR_FAT_FINGER_BASE_PRICE_ZEROED = "Base price zeroed for volume calculation";
        public static string ERR_OMS_SENDING_ORDER = "Client is not allowed to send orders in OMS. Client account: {0}";
        public static string ERR_TEST_INSTRUMENT_NOT_FOUND = "Test instrument not found";
        public static string ERR_INSTITUTIONAL_PROFILE_FOUND = "Client with institutional profile found. Client account: {0}";
        public static string ERR_GIVE_UP_ACCOUNT = "Client with give-up account found. Client account: {0}";
        public static string ERR_SPIDER_PERMISSION = "Client is not allowed to send orders in OMS Spider. Client account: {0}";

        public static string ERR_LIMIT_FIX_NOT_FOUND = "Fix limit not found. IdFix: {0}";
        public static string ERR_LIMIT_FIX_FAT_FINGER = "The order sent exceeded the maximum limit allowed per fix session";
        public static string ERR_LIMIT_FIX_EXCEEDS = "Client does not have sufficient fix session limit to send this order";

        public static string ERR_MAX_LOSS_EXCEEDED = "Client does not have enough maximum loss limit to send this order";

    }
}
