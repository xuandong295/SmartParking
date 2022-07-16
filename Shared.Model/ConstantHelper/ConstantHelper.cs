using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.ConstantHelper
{
    public class ConstantHelper
    {
        public struct InternalAPIResponseCode
        {

            /// <summary>
            /// this is presentation code of internal api communicate
            /// <code>0. resource not found </code>
            /// <code>1. excute resource successed </code>
            /// <code>2. write resource forbidden </code>
            /// <code>3. read resource forbidden </code>
            /// <code>4. internal serve error </code>
            /// <code>5. conflict resource </code>
            /// <code>6. license not yet active </code>
            /// <code>7. user disable </code>
            /// <code>8. license is lock down </code>
            /// <code>9. resource of cloudtype not support </code>
            /// <code>10. User invation </code>
            /// <code>11. Resource already exists </code>
            /// </summary>
            public int Code { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }
        }
        public struct MessageAPIResponse
        {
            public static readonly string NOT_ENOUGH_MONNEY = "User have not enough monney please recharge and try again";
            public static readonly string ATTACHED_RG = "Some cloud accounts are attached to this resource group.";
            public static readonly string RESOURCE_UPDATE_FAILED = "Update resource unsuccessfully";
            public static readonly string SUPSCRIPTION_ALREADY_ACTIVE = "Subscription already activated";
            public static readonly string LICENSE_NOT_EXISTS = "The license key does not exist";
            public static readonly string SUBSCRIPTION_NOT_EXISTS = "The subscription does not exist";
            public static readonly string USERNAME_ALREADY_EXISTS = "The username already exist";
            public static readonly string LICENSE_ALREADY_ACTIVE = "The license key has already been used";
            public static readonly string LICENSE_ACTIVE_SUCCESSED = "Activate license key successfully";
            public static readonly string LICENSE_INVALID = "Invalid license key";
            public static readonly string UPDATE_USER_ERROR = "User update error";
            public static readonly string SUBSCRIPTION_INVALID = "Invalid subscription";
            public static readonly string LICENSE_EXPIRED = "The license key has expired";
            public static readonly string SUBSCRIPTION_EXPIRED = "The subscription has expired";
            public static readonly string LICENSE_NOT_START = "The license key start time is in the future";
            public static readonly string SUBSCRIPTION_NOT_START = "The subscription start time is in the future";
            public static readonly string LICENSE_GENERATE_SUCCESSED = "Generate license key successfully";
            public static readonly string TOKEN_INVALID = "Invalid or expired token"; // + unauthorized
            public static readonly string ACCESS_DENIED = "Access denied";
            public static readonly string INVALID_PARAMETER = "Invalid parameters";
            public static readonly string PARAMETER_PAGE_OR_LIMIT_INVALID = "Parameter page or limit invalid.";
            public static readonly string CUSTOMER_ALREADY_EXISTS = "Customer already exists";
            public static readonly string ADMIN_USER_ALREADY_EXISTS = "Admin user already exists";
            public static readonly string CANNOT_CONNECT_TO_ENDPOINT = "Cannot connect to endpoint";
            public static readonly string INVALID_EMAIL = "Invalid email";
            public static readonly string CANNOT_CONNECT_STS_ACCOUNT = "Cannot connect by cross-account access";
            public static readonly string USER_INFOR_IS_NOT_VALID = "Invalid user information";
            public static readonly string CANNOT_DISABLE_YOUR_SELF = "Can not disable yourself";
            public static readonly string CLOUD_ACCOUNT_IS_DISABLE = "Can not audit for cloud account, reason: status cloud not enable";
            public static readonly string CLOUD_ACCOUNT_INCORRECT = "Can not audit for cloud account, reason: cloud account incorrect or not not found";
            public static readonly string CANNOT_INVITE_USER = "Cannot invite user";
            public static readonly string INVITE_USER_SUCESSFULLY = "Invite user sucessfully";
            public static readonly string USER_ALREADY_EXIST_IN_DB = "User already exists";
            public static readonly string USER_ALREADY_EXIST_IN_SUB = "User are already in this subscription";
            public static readonly string RESOURCE_NOT_FOUND = "Resource not found";
            public static readonly string MISSING_API_TOKEN_PARAM = "Missing or malformed 'Authorization' header";
            public static readonly string API_KEY_INVALID_OR_EXPIRED = "API Key invalid or expired";
            public static readonly string INVALID_DATE_TIME_REQUEST_API_KEY = "Start date cannot be in the past"; //
            public static readonly string MAXIMUM_RESOURCES_ALLOW = "Cannnot generate more than 2 access keys";
            public static readonly string CLOUD_TYPE_UNSUPPORTED = "Unsupported cloud type";
            public static readonly string RESOURCE_ALREADY_EXISTS = "Resource already exists";
            public static readonly string SAME_SUB = "You are already in this subscription";
            public static readonly string RESOURCE_ALREADY_REMOVE = "Delete resource successfully";
            public static readonly string ROLE_NOT_EXISTS = "Role not exists";
            //public static readonly string AUTHOR_FOR_KEY = "License key invalid for your subscription";
            public static readonly string WRITE_ACCESS_FORBIDDEN = "Write access forbidden";
            public static readonly string QUERY_SUCCESSED = "OK";
            public static readonly string LICENSE_KEY_NOT_EXISTS = "Licensen key does not exist";
            public static readonly string SUBSCRIPTION_NOT_YET_ACTIVE = "Subscription not yet activated";
            public static readonly string USER_DISABLE = "User disabled";
            public static readonly string SUBSCRIPTION_ACCESS_DENY = "Access deny the current subscription";
            public static readonly string RESOURCE_CREATE_SUCCESSED = "Create resource successfully";
            public static readonly string RESOURCE_CREATE_ERROR = "Create resource error";
            public static readonly string RESOURCE_UPDATE_SUCCESSED = "Update resource successfully";
            public static readonly string CLOUD_ACC_CANNOT_BE_REMOVED_FROM_AG = "Cannot remove cloud accounts from an account group";
            public static readonly string ACC_UPDATE_SUCCESSED = "Update account successfully";
            public static readonly string STATUS_UPDATE_SUCCESSED = "Update account successfully";
            public static readonly string RESOURCE_DELETE_SUCCESSED = "Delete resource successfully";
            public static readonly string LICENSE_IS_LOCKDOWN = "License is lockdown";
            public static readonly string USER_INVATION = "Invite user successfully";
            public static readonly string USER_REMOVE_SUCCESSED = "Delete user successfully";
            public static readonly string USER_DISABLE_SUCCESSED = "Update user status successfully";
            public static readonly string USER_UPDATE_SUCCESSED = "Update user successfully";
            public static readonly string UPDATE_LANGUAGE_SUCCESSED = "Update language successfully";
            public static readonly string USER_NOT_EXISTS = "The user does not exist";
            public static readonly string QUOTA_OVERLOAD = "Quota of workload cannot be over 2000";
            public static readonly string OK = "OK";
            public static readonly string NOT_OK = "NOT OK";
            public static readonly string READY_TO_SCAN = "Ready to scan cloud account";
            public static readonly string UNSUPPORTED_MEDIA_TYPE = "Unsupported media type";
            public static readonly string EDIT_ACC_SUC = "Edit account successfully";
            public static readonly string AUTHOR_FOR_KEY = "License Incorrect";
            public static readonly string CLOUD_NAME_ALREADY_EXISTS = "Cloud name already exists";
            public static readonly string CLOUD_NAME_NOT_EXISTS = "Cloud name not exists in subscription";
            public static readonly string ACCOUNTGROUP_NAME_ALREADY_EXISTS = "Account group name already exists";
            public static readonly string ACCOUNTGROUP_NOT_EXISTS = "Account group does not exists";
            public static readonly string CREATE_ACCOUNT_CLOUD_SUCCESSED = "Create cloud account successefully";
            public static readonly string CLOUD_ACCOUNT_INVALID = "Cloud account is invalid";
            public static readonly string CAN_NOT_CREATE_CLOUD_ACCOUNT = "Cannot create cloud account";
            public static readonly string CREATE_COMPLIANCE_SUCESSFULLY = "Create compliance successfully";
            public static readonly string FILTER_ACCESS_DENIED = "Filter access denied";
            public static readonly string NO_DATA_FOUND = "No data found";
            public static readonly string REGION_NOT_EXIST = "Region not exists";
            public static readonly string NO_SUCH_SERVICE_OR_RESOURCE = "No data found";
            public static readonly string PERMISSION_INFOR_NOT_VALID = "Invalid permission information";
            public static readonly string PERMISSION_NOT_FOUND = "Permission not found";
            public static readonly string ROLE_INFO_NOT_VALID = "Invalid role information";
            public static readonly string SUBSCRIPTION_PACKAGES_INFOR_NOT_VALID = "Invalid subscription packages information";
            public static readonly string SUBSCRIPTION_PACKAGES_NOT_FOUND = "Subscription packages not found";
            public static readonly string WORKLOAD_NOT_FOUND = "Workload not found";
            public static readonly string PLEASE_INSERT_FILE = "Please insert file";
            public static readonly string IMPORT_FAILED = "Import failed";
            public static readonly string SUB_HAS_NO_USERS = "Subscription has no users";
            public static readonly string CANT_CONNECT_ACC = "Cannot connect to account";
            public static readonly string CHECK_CLOUD_SUCCESSEDFULLY = "Check cloud successfully";
            public static readonly string CAN_NOT_VALIDATE_CLOUD_ACCOUNT = "Cannot verify cloud account";
            public static readonly string NOT_FOUND_GROUP_OR_ACC = "Account group or cloud account not found";
            public static readonly string WRONG_COMPLIANCE_REPORT = "Invalid compliance for the cloud account";
            public static readonly string INVESTIGATE_TYPE_ERR = "Type must be investigate";
            public static readonly string COMPLIANCE_NOT_FOUND = "Compliance not found";
            public static readonly string REQUIREMENT_NOT_FOUND = "Requirement not found";
            public static readonly string SECTION_NOT_FOUND = "Section not found";
            public static readonly string INVALID_ACCOUNT_INFOR = "Invalid account information";
            public static readonly string QUERY_MUST_HAVE_VALUE = "Query must have value";
            public static readonly string CLOUD_INFO_EMPTY = "Information for cloud account cannot be empty";
            public static readonly string CLOUD_NAME_EMPTY = "Cloud account name cannot be empty";
            public static readonly string INVALID_SHEET_NAME = "Invalid sheet name";
            public static readonly string ACCOUNT_ALREADY_IMPORTED = "Cloud account already imported";
            public static readonly string SCAN_TRIGGER_SENT = "Scan trigger sent";
            public static readonly string ERROR = "Error";
            public static readonly string CANNOT_SEND_EMAIL = "Cannot send invitation email";
            public static readonly string CLOUD_ACCOUNT_SERVICES_NOT_YET_READY = "Cloud account services not yet ready";
            public static readonly string PERMISSION_DENIED = "Permission denined";
            public static readonly string REQUEST_SCAN_CLI = "Create the request successed";
            public static readonly string REGION_NOT_EXISTS = "Region not exists";
            public static readonly string INTERNAL_SERVER_ERROR = "Interal server error";
            public static readonly string THE_LETEST_STILL_WORKING = "Already exists the request and still running. Can not process your request";
            public static readonly string INVALID_PARAMETERS_FOR_CLI_REQUEST = "The input parameters for CLI request is missing or invalid or not matching format, please check your input parameters";
            public static readonly string INVALID_ROLE_ARN = "Invalid Role ARN";
            public static readonly string INVALID_ACCESS_KEY_AND_SECRET_KEY_OR_ROLE_ARN_AND_EXTERNAL_ID = "Invalid Access Key and SecretKey or Role ARN and External ID";
            public static readonly string INVALID_TIME_DURATION = "Invalid time duration";
            public static readonly string UPDATE_RESOURCE_SUCCESS = "Update resoure successfull!";
            public static readonly string INVALID_WORKLOAD_LIMIT = "Invalid workloads limit";
            public static readonly string INVALID_START_EXPIRE_TIME = "Invalid start time or expire time";
            public static readonly string SCHEDULE_TIME_NOT_EXISTS = "Schedule time not exists ";
            public static readonly string ID_NOT_EXISTS = "Id not exists";
            public static readonly string CONFIG_POLICY_NOT_EXIST = "Config policy not exists";
            public static readonly string EXCEPTION_NOT_EXIST = "Exception not exists";
            public static readonly string POLICY_NOT_EXIST_IN_SUB = "Your subscription does not have this policy";
            public static readonly string USER_NOT_EXIST_IN_SUB = "Your subscription does not have this user";
            public static readonly string ROLE_NOT_EXIST_IN_SUB = "Your subscription does not have this role";
            public static readonly string REGION_NOT_EXIST_IN_SUB = "Your subscription does not have this region";
            public static readonly string CLOUD_ACCOUNT_NOT_IN_SUB = "Cloud account not in subscription";
            public static readonly string CAN_NOT_UPDATE_DECTECTION_POLICIES = "Can not update detection policies";
            public static readonly string ALERTCHANNEL_ALREADY_EXISTS = "Alert channel is attached to some alert rule(s)";
            public static readonly string DUPLICATE_CLOUD_ACCOUNT = "Duplicate cloud account";
            public static readonly string JIRA_CUSTOM_TEMPLATE_MUST_IN_JSON = "Jira custom template must be in json format";
            public static readonly string MUST_HAVE_FIELDS_PROPERTY_IN_CUSTOM_TEMPLATE = "\"fields\" property in custom template must exist and can not be null or empty";
            public static readonly string INVALID_URL = "Invalid URL";
            public static readonly string NO_SUCH_HOST_IS_KNOWN = "No such host is known";
            public static readonly string CLOUD_ACCOUNT_ALREADY_EXISTS = "Cloud Acccount already exists in subscription";
            public static readonly string ONLY_INPUT_KEY_OR_ARN = "Can only input acccess key or role arn";
            public static readonly string CREDENTIAL_ALREADY_EXISTS = "This credentials already exists on a SoraTrust cloud account. Please log into SoraTrust to get the existing cloud account name and use it to scan.";
            public static readonly string ONLY_TEST_INTEGRATION_JIRA_SDP_WEBHOOK_AWSSECURITYHUB = "Test function doesn't support for email integration";
            public static readonly string TEST_INTEGRATION_FAIL = "Test integration failed";
            public static readonly string TOKEN_KEY_EXPIRED = "The token key has expired";
            public static readonly string OFFICE365_POLICY_NOT_FOUND = "Office365 policy not found";
        }
        public struct APICodeResponse
        {
            public static readonly int SUCCESSED_CODE = 0;
            public static readonly int FAILED_CODE = 1;
            public static readonly int INTERNAL_SERVER_CODE = 2;
            public static readonly int RESOURCE_CONFLICT = 3;
            public static readonly int LICENSE_NOT_ACTIVE = 6;
            public static readonly int USER_DISABLED = 7;
            public static readonly int CLIENT_ERROR = 8;

        }

        public struct Role
        {
            public static readonly string ADMIN = "Admin";
            public static readonly string USER = "User";
        }
    }
}
