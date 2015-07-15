// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameValueLists.cs">
//   Copyright (C) 2015 by Alexander Davyduk. All rights reserved.
// </copyright>
// <summary>
//   Defines the NameValueDroplist class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.SharedSource.CustomFields.Fields
{
  using System;
  using System.Collections.Specialized;
  using System.Linq;
  using System.Web;
  using System.Web.UI;
  using Sitecore.Configuration;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.StringExtensions;
  using Sitecore.Text;
  using Sitecore.Web.UI.HtmlControls;

  /// <summary>
  /// Defines NameValueLists class
  /// </summary>
  public sealed class NameValueDroplists : Input
  {
    /// <summary>
    /// The source
    /// </summary>
    private string source;

    /// <summary>
    /// The item id
    /// </summary>
    private string itemId;

    /// <summary>
    /// Initializes a new instance of the <see cref="NameValueDroplists"/> class.
    /// </summary>
    public NameValueDroplists()
    {
      this.Class = "scContentControl";
      this.Activation = true;
    }

    /// <summary>
    /// Gets or sets the item ID.
    /// </summary>
    /// <value>
    /// The item ID.
    /// </value>
    public string ItemId
    {
      get
      {
        return StringUtil.GetString(new[]
        {
          this.itemId
        });
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");
        this.itemId = value;
      }
    }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    /// <value>
    /// The source.
    /// </value>
    public string Source
    {
      get
      {
        return StringUtil.GetString(new[]
        {
          this.source ?? (string)this.ServerProperties["_source"]
        });
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");
        this.source = value;
        
        this.ServerProperties["_source"] = this.source;
      }
    }

    /// <summary>
    /// Gets the key source.
    /// </summary>
    /// <value>
    /// The key source.
    /// </value>
    public string KeySource
    {
      get
      {
        if (string.IsNullOrEmpty(this.Source))
        {
          return string.Empty;
        }

        var sources = this.Source.Split('|');

        if (sources.Count() != 0)
        {
          return sources[0];
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Gets the value source.
    /// </summary>
    /// <value>
    /// The value source.
    /// </value>
    public string ValueSource
    {
      get
      {
        if (string.IsNullOrEmpty(this.Source))
        {
          return string.Empty;
        }

        var sources = this.Source.Split('|');

        if (sources.Count() == 2)
        {
          return sources[1];
        }

        return string.Empty;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is vertical.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is vertical; otherwise, <c>false</c>.
    /// </value>
    private bool IsVertical
    {
      get
      {
        return false;
      }
    }

    /// <summary>
    /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object, which writes the content to be rendered on the client.
    /// </summary>
    /// <param name="output">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.</param>
    protected override void DoRender(HtmlTextWriter output)
    {
      Assert.ArgumentNotNull(output, "output");
      this.SetWidthAndHeightStyle();
      output.Write("<div" + this.ControlAttributes + ">");
      this.RenderChildren(output);
      output.Write("</div>");
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
    protected override void OnLoad(EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");
      base.OnLoad(e);
      if (Sitecore.Context.ClientPage.IsEvent)
      {
        this.LoadValue();
      }
      else
      {
        this.BuildControl();
      }
    }

    /// <summary>
    /// Sets the modified flag.
    /// </summary>
    protected override void SetModified()
    {
      base.SetModified();
      if (this.TrackModified)
      {
        Sitecore.Context.ClientPage.Modified = true;
      }
    }

    /// <summary>
    /// Builds the control.
    /// </summary>
    private void BuildControl()
    {
      var keys = new UrlString(this.Value);
      foreach (var key in keys.Parameters.Keys.Cast<string>().Where(key => key.Length > 0))
      {
        this.Controls.Add(this.BuildParameterKeyValue(key, HttpUtility.UrlDecode(keys.Parameters[key])));
      }

      this.Controls.Add(this.BuildParameterKeyValue(string.Empty, string.Empty));
    }

    /// <summary>
    /// Initializes the select.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="dataSource">The data source.</param>
    /// <returns>
    /// Initialized select with options
    /// </returns>
    private string InitializeSelect(string key, string dataSource)
    {
      var options = this.AddOption(string.Empty, string.Empty);

      if (string.IsNullOrEmpty(this.Source))
      {
        return options;
      }

      var sourceItems = new Item[]
      {
      };

      if (!dataSource.StartsWith("query:", StringComparison.InvariantCulture))
      {
        var sourceItem = Factory.GetDatabase("master").GetItem(dataSource);
        if (sourceItem != null)
        {
          sourceItems = sourceItem.Children.ToArray();
        }
      }
      else
      {
        sourceItems = Factory.GetDatabase("master").SelectItems(dataSource.Substring("query:".Length));
      }

      return sourceItems.Aggregate(options, (current, item) => current + this.AddOption(item.ID.ToString(), item.Name, key == item.ID.ToString()));
    }

    /// <summary>
    /// Adds the option.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="text">The text.</param>
    /// <param name="selected">if set to <c>true</c> [selected].</param>
    /// <returns>formatted option</returns>
    private string AddOption(string value, string text, bool selected = false)
    {
      return selected ? string.Format("<option value=\"{0}\" selected>{1}</option>", value, text) : string.Format("<option value=\"{0}\">{1}</option>", value, text);
    }

    /// <summary>
    /// Builds the parameter key value.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    /// <returns>controls border</returns>
    private Border BuildParameterKeyValue(string key, string value)
    {
      Assert.ArgumentNotNull(key, "key");
      Assert.ArgumentNotNull(value, "value");

      var border = new Border();

      var uniqueId = GetUniqueID(this.ID + "_Param");
      Sitecore.Context.ClientPage.ServerProperties[this.ID + "_LastParameterID"] = uniqueId;
      var clientEvent = Sitecore.Context.ClientPage.GetClientEvent(this.ID + ".ParameterChange");
      var readOnlyParameter = this.ReadOnly ? " readonly=\"readonly\"" : string.Empty;
      var disabledParameter = this.Disabled ? " disabled=\"disabled\"" : string.Empty;
      var verticalParameter = this.IsVertical ? "</tr><tr>" : string.Empty;

      var keySelectControl = string.Format(
        "<select style='width:100%;' id=\"{0}\" name=\"{1}\" type=\"text\"{2}{3} value=\"{4}\" onchange=\"{5}\">{6}</select>",
        uniqueId,
        uniqueId,
        readOnlyParameter,
        disabledParameter,
        StringUtil.EscapeQuote(key),
        clientEvent,
        this.InitializeSelect(key, this.KeySource));

      var valueSelectControl = string.Format(
        "<select style='width:100%;' id=\"{0}_value\" name=\"{1}_value\" type=\"text\"{2}{3} value=\"{4}\" onchange=\"{5}\">{6}</select>",
        uniqueId,
        uniqueId,
        readOnlyParameter,
        disabledParameter,
        StringUtil.EscapeQuote(value),
        clientEvent,
        this.InitializeSelect(value, this.ValueSource));

      border.Controls.Add(new LiteralControl("<table id=\"{0}\" width=\"100%\" cellpadding=\"4\" cellspacing=\"0\" border=\"0\"><tr><td class=\"test\" width=\"50%\">".FormatWith(uniqueId + "_table")));
      border.Controls.Add(new LiteralControl(keySelectControl));
      border.Controls.Add(new LiteralControl("</td>"));
      border.Controls.Add(new LiteralControl(verticalParameter));
      border.Controls.Add(new LiteralControl("<td width=\"50%\">"));
      border.Controls.Add(new LiteralControl(valueSelectControl));
      
      border.Controls.Add(new LiteralControl("</td></tr></table>"));

      return border;
    }

    /// <summary>
    /// Loads the value.
    /// </summary>
    private void LoadValue()
    {
      if (this.ReadOnly || this.Disabled)
      {
        return;
      }

      var handler = HttpContext.Current.Handler as Sitecore.Web.UI.HtmlControls.Page;
      var form = handler != null ? handler.Request.Form : new NameValueCollection();
      var list = new UrlString();
      foreach (string key in form.Keys)
      {
        if ((string.IsNullOrEmpty(key) || !key.StartsWith(this.ID + "_Param", StringComparison.InvariantCulture)) || key.EndsWith("_value", StringComparison.InvariantCulture))
        {
          continue;
        }

        var name = form[key];
        var value = form[key + "_value"];
        if (!string.IsNullOrEmpty(name))
        {
          list[name] = value ?? string.Empty;
        }
      }

      var fieldValue = list.ToString();
      if (this.Value == fieldValue)
      {
        return;
      }

      this.Value = fieldValue;
      this.SetModified();
    }

    /// <summary>
    /// Parameters the change.
    /// </summary>
    [UsedImplicitly]
    private void ParameterChange()
    {
      var clientPage = Sitecore.Context.ClientPage;
      var value = clientPage.ClientRequest.Form[clientPage.ClientRequest.Source];
      if (!string.IsNullOrEmpty(value))
      {
        if (clientPage.ClientRequest.Source == StringUtil.GetString(clientPage.ServerProperties[this.ID + "_LastParameterID"]))
        {
          var border = this.BuildParameterKeyValue(string.Empty, string.Empty);
          clientPage.ClientResponse.Insert(this.ID, "beforeEnd", border);
        }
      }
      else
      {
        if (!clientPage.ClientRequest.Source.EndsWith("_value"))
        {
          clientPage.ClientResponse.Remove(clientPage.ClientRequest.Source + "_table");
        }
      }

      NameValueCollection form = null;
      var handler = HttpContext.Current.Handler as Sitecore.Web.UI.HtmlControls.Page;
      if (handler != null)
      {
        form = handler.Request.Form;
      }

      if (form != null)
      {
        clientPage.ClientResponse.SetReturnValue(true);
      }
    }
  }
}