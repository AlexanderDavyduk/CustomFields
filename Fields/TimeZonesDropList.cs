// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeZonesDropList.cs">
//   Copyright (C) 2015 by Alexander Davyduk. All rights reserved.
// </copyright>
// <summary>
//   Defines the time zones drop list class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.SharedSource.CustomFields.Fields
{
  using System;
  using System.Collections.ObjectModel;
  using System.Web.UI;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;

  /// <summary>
  /// Defines the time zones drop list class.
  /// </summary>
  public class TimeZonesDropList : Web.UI.HtmlControls.Control
  {
    private string _fieldname = string.Empty;
    private bool _hasPostData;
    private string _itemid;
    private string _source = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="TimeZonesDropList"/> class.
    /// </summary>
    public TimeZonesDropList()
    {
      // this.Class = "scContentControl";
      this.Activation = true;
    }

    /// <summary>
    /// Sends server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter"></see> object, which writes the content to be rendered on the client.
    /// </summary>
    /// <param name="output">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> object that receives the server control content.</param>
    protected override void DoRender(HtmlTextWriter output)
    {
      Assert.ArgumentNotNull(output, "output");
      var items = this.GetItems();
      output.Write("<select" + this.GetControlAttributes() + ">");
      output.Write("<option value=\"\"></option>");
      bool flag = false;
      foreach (var item in items)
      {
        string itemHeader = item.DisplayName;
        bool flag2 = this.IsSelected(item);
        if (flag2)
        {
          flag = true;
        }

        output.Write("<option value=\"" + item.Id + "\"" + (flag2 ? " selected=\"selected\"" : string.Empty) + ">" + itemHeader + "</option>");
      }
      bool flag3 = !string.IsNullOrEmpty(this.Value) && !flag;
      if (flag3)
      {
        output.Write("<optgroup label=\"" + Translate.Text("Value not in the selection list.") + "\">");
        output.Write("<option value=\"" + this.Value + "\" selected=\"selected\">" + this.Value + "</option>");
        output.Write("</optgroup>");
      }
      output.Write("</select>");
      if (flag3)
      {
        output.Write("<div style=\"color:#999999;padding:2px 0px 0px 0px\">{0}</div>", Translate.Text("The field contains a value that is not in the selection list."));
      }
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <returns>The items.</returns>
    protected virtual ReadOnlyCollection<TimeZoneInfo> GetItems()
    {    
      return TimeZoneInfo.GetSystemTimeZones();
    }

    /// <summary>
    /// Determines whether the specified item is selected.
    /// </summary>
    /// <param name="item">The item .</param>
    /// <returns>
    /// true if the specified item is selected; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool IsSelected(TimeZoneInfo item)
    {
      Assert.ArgumentNotNull(item, "item");
      if (this.Value != item.Id)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Loads the post data.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// Returns <c>true</c>, if a change has occurred, otherwise <c>false</c>.
    /// </returns>
    protected override bool LoadPostData(string value)
    {
      this._hasPostData = true;
      if (value == null)
      {
        return false;
      }

      if (this.GetViewStateString("Value") != value)
      {
        SetModified();
      }

      this.SetViewStateString("Value", value);
      return true;
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Load"></see> event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.EventArgs"></see> object that contains the event data.</param>
    protected override void OnLoad(EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");
      base.OnLoad(e);
      if (!this._hasPostData)
      {
        this.LoadPostData(string.Empty);
      }
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnPreRender(EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");
      base.OnPreRender(e);
      this.ServerProperties["Value"] = this.ServerProperties["Value"];
    }

    /// <summary>
    /// Sets the Modified flag.
    /// </summary>
    private static void SetModified()
    {
      Sitecore.Context.ClientPage.Modified = true;
    }

    /// <summary>
    /// Gets or sets the name of the field.
    /// </summary>
    /// <value>The name of the field.</value>
    public string FieldName
    {
      get
      {
        return this._fieldname;
      }
      set
      {
        Assert.ArgumentNotNull(value, "value");
        this._fieldname = value;
      }
    }

    /// <summary>
    /// Gets or sets the item ID.
    /// </summary>
    /// <value>The item ID.</value>
    public string ItemID
    {
      get
      {
        return this._itemid;
      }
      set
      {
        Assert.ArgumentNotNull(value, "value");
        this._itemid = value;
      }
    }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    /// <value>The source.</value>
    public string Source
    {
      get
      {
        return this._source;
      }
      set
      {
        Assert.ArgumentNotNull(value, "value");
        this._source = value;
      }
    }
  }
}