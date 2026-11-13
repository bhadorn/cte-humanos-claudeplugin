using HumanOS.Kernel;
using HumanOS.Kernel.DataModel;
using HumanOS.Kernel.PeSeL.DataLogger;
using HumanOS.Kernel.PeSeL.Script;
using HumanOS.Kernel.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace HumanOS.PeSeL.NodeSpaceDataLogger.Script
{
  /// <summary>
  /// Builds the platform stream payload (version 2) for the stream "HallTemperature":
  /// one message per data node, RefId = id of the Hall entity (node property HallId).
  /// </summary>
  public class THallTemperaturePayload : TAbstractDataLoggerScriptObject<string>
  {
    public override void initialize(IKernelAccess Kernel, TPayloadProcessingContext Context)
    {
    }

    public override void postProcess(IKernelAccess Kernel, TPayloadProcessingContext Context)
    {
    }

    public override string[] processPayload(IKernelAccess Kernel, TPayloadProcessingContext Context, List<TDataSet> lstData)
    {
      JArray jMessages = new JArray();
      foreach (TDataSet DataSet in lstData)
      {
        Guid HallId = DataSet.getFieldValue<Guid>("HallId");
        if (HallId == Guid.Empty)
        {
          Logger.writeError($"Node '{DataSet.getFieldValue<string>("Name")}' has no HallId property, skipped.");
          continue;
        }

        double dTemperature;
        try
        {
          dTemperature = Convert.ToDouble(DataSet.getFieldValue<object>("Value"));
        }
        catch (Exception Ex)
        {
          Logger.writeError($"Value of '{DataSet.getFieldValue<string>("Name")}' is not a number: {Ex.Message}");
          continue;
        }

        DateTime TimeStamp = DataSet.getFieldValue<DateTime>("TimeStamp").ToUniversalTime();
        JObject jFields = new JObject();
        jFields.Add("Temperature", dTemperature);
        jFields.Add("TimeStamp", TimeStamp.ToString("o"));

        JObject jMessage = new JObject();
        jMessage.Add("Stream", DataSet.Name);                        // = StreamModel.Name on the platform
        jMessage.Add("RefId", HallId);                               // = Hall.Id
        jMessage.Add("TimeStamp", TimeStamp.ToString("o"));
        jMessage.Add("State", DataSet.getFieldValue<int>("State"));  // 1 = Good
        jMessage.Add("Fields", jFields);
        jMessages.Add(jMessage);
      }

      JObject jRoot = new JObject();
      jRoot.Add("Messages", jMessages);
      return new string[] { jRoot.ToString() };
    }
  }
}
