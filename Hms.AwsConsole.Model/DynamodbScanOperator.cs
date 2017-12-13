using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hms.AwsConsole.Model
{
    public class DynamodbScanOperator
    {
        //readonly DynamodbScanOperator
        //
        // Summary:
        //     Constant BEGINS_WITH for DynamodbScanOperator
        public static string BEGINS_WITH;
        //
        // Summary:
        //     Constant BETWEEN for DynamodbScanOperator
        public static string BETWEEN;
        //
        // Summary:
        //     Constant CONTAINS for DynamodbScanOperator
        public static string CONTAINS;
        //
        // Summary:
        //     Constant EQ for DynamodbScanOperator
        public static string EQ { get { return "EQ"; } }
        //
        // Summary:
        //     Constant GE for DynamodbScanOperator
        public static string GE;
        //
        // Summary:
        //     Constant GT for DynamodbScanOperator
        public static string GT;
        //
        // Summary:
        //     Constant IN for DynamodbScanOperator
        public static string IN;
        //
        // Summary:
        //     Constant LE for DynamodbScanOperator
        public static string LE;
        //
        // Summary:
        //     Constant LT for DynamodbScanOperator
        public static string LT;
        //
        // Summary:
        //     Constant NE for DynamodbScanOperator
        public static string NE;
        //
        // Summary:
        //     Constant NOT_CONTAINS for DynamodbScanOperator
        public static string NOT_CONTAINS;
        //
        // Summary:
        //     Constant NOT_NULL for DynamodbScanOperator
        public static string NOT_NULL;
        //
        // Summary:
        //     Constant NULL for DynamodbScanOperator
        public static string NULL;

        //
        // Summary:
        //     This constant constructor does not need to be called if the constant you are
        //     attempting to use is already defined as a static instance of this class. This
        //     constructor should be used to construct constants that are not defined as statics,
        //     for instance if attempting to use a feature that is newer than the current version
        //     of the SDK.
        //public DynamodbScanOperator(string value);

        //
        // Summary:
        //     Finds the constant for the unique value.
        //
        // Parameters:
        //   value:
        //     The unique value for the constant
        //
        // Returns:
        //     The constant for the unique value
        //public static DynamodbScanOperator FindValue(string value);

        //
        // Summary:
        //     Utility method to convert strings to the constant class.
        //
        // Parameters:
        //   value:
        //     The string value to convert to the constant class.
        //public static implicit operator DynamodbScanOperator(string value);
    }
}
