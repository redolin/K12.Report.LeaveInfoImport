using Campus.DocumentValidator;
using K12.Report.LeaveInfoImport.ValidationRule.FieldValidator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Report.LeaveInfoImport.ValidationRule
{
    class LeaveInfoFieldValidatorFactory : IFieldValidatorFactory
    {
        #region IRowValidatorFactory 成員

        IFieldValidator IFieldValidatorFactory.CreateFieldValidator(string typeName, System.Xml.XmlElement validatorDescription)
        {
            switch (typeName.ToUpper())
            {
                case "K12REPORTLEAVEINFOIMPORTCHECKSTUDENTNUMBER":
                    return new StudentInischoolCheck();
                default:
                    return null;
            }
        }

        #endregion
    }
}
