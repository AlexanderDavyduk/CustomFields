// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortableMultilist.cs">
//   Copyright (C) 2015 by Alexander Davyduk. All rights reserved.
// </copyright>
// <summary>
//   Defines the SortableMultilist type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.SharedSource.CustomFields.Fields
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Web.UI;
  using Sitecore.Data.Items;
  using Sitecore.Diagnostics;
  using Sitecore.Globalization;
  using Sitecore.Shell.Applications.ContentEditor;
  using Sitecore.StringExtensions;
  using Sitecore.Text;
  using Sitecore.Web.UI.HtmlControls.Data;
  using Sitecore.Web.UI.Sheer;

  /// <summary>
  /// Defines Sortable multi list class
  /// </summary>
  public class SortableMultilist : Sitecore.Web.UI.HtmlControls.Control, IContentField
  {
    /// <summary>
     /// The item id
     /// </summary>
    private string itemid;
    
    /// <summary>
    /// The source
    /// </summary>
    private string source;

    /// <summary>
    /// Initializes a new instance of the <see cref="SortableMultilist"/> class.
    /// </summary>
    public SortableMultilist()
    {
      this.Class = "scContentControlSortableMultilist";
      this.Activation = true;
      this.source = string.Empty;
    }

    /// <summary>
    /// Gets or sets the item ID.
    /// </summary>
    /// <value>
    /// The item ID.
    /// </value>
    /// <contract><requires name="value" condition="not null"/><ensures condition="nullable"/></contract>
    public string ItemId
    {
      get
      {
        return this.itemid;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");
        this.itemid = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the field is read only.
    /// </summary>
    /// <value>
    /// <c>true</c> if the field is read only; otherwise, <c>false</c>.
    /// </value>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    /// <value>
    /// The source.
    /// </value>
    /// <contract><requires name="value" condition="not null"/><ensures condition="nullable"/></contract>
    public string Source
    {
      get
      {
        return this.source;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");
        this.source = value;
      }
    }

    /// <summary>
    /// Gets or sets the item language.
    /// </summary>
    /// <value>
    /// The item language.
    /// </value>
    public string ItemLanguage
    {
      get
      {
        return StringUtil.GetString(this.ViewState["ItemLanguage"]);
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");
        this.ViewState["ItemLanguage"] = value;
      }
    }

    /// <summary>
    /// Handles the message.
    /// </summary>
    /// <param name="message">The message.</param>
    public override void HandleMessage(Message message)
    {
      Assert.ArgumentNotNull(message, "message");
      base.HandleMessage(message);
      if (message["id"] != this.ID)
      {
        return;
      }

      switch (message.Name)
      {
        case "contentmultilist:selectall":
          SheerResponse.Eval("SortableMultilistSelectAll('" + this.ID + "')");
          break;
        case "contentmultilist:unselectall":
          SheerResponse.Eval("SortableMultilistDeselectAll('" + this.ID + "', true)");
          break;
      }
    }
    
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <returns>
    /// The value of the field.
    /// </returns>
    public string GetValue()
    {
      return this.Value;
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="value">The value.</param>
    public void SetValue(string value)
    {
      Assert.ArgumentNotNull(value, "value");
      this.Value = value;
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
    /// </summary>
    /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnLoad(EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");
      base.OnLoad(e);
      var str = Sitecore.Context.ClientPage.ClientRequest.Form[this.ID + "_value"];
      if (str == null)
      {
        return;
      }

      if (this.GetViewStateString("Value", string.Empty) != str)
      {
        this.SetModified();
      }

      this.SetViewStateString("Value", str);
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnPreRender(EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");
      base.OnPreRender(e);
    }

    /// <summary>
    /// Renders the control.
    /// </summary>
    /// <param name="output">The output.</param>
    protected override void DoRender(HtmlTextWriter output)
    {
      Assert.ArgumentNotNull(output, "output");
      var current = Sitecore.Context.ContentDatabase.GetItem(this.ItemId, Language.Parse(this.ItemLanguage));
      Item[] sources;
      using (new LanguageSwitcher(this.ItemLanguage))
      {
        sources = LookupSources.GetItems(current, this.Source);
      }

      ArrayList selected;
      IDictionary unselected;
      this.GetSelectedItems(sources, out selected, out unselected);
      this.ServerProperties["ID"] = this.ID;
     
      output.Write("<input id=\"" + this.ID + "_Value\" type=\"hidden\" value=\"" + StringUtil.EscapeQuote(this.Value) + "\" />");
      output.Write("<div class='scContentControlMultilistContainer'>");

      output.Write("<div class=\"select-lebel\">Fast Sorting:</div>");
      var selectControl = string.Format(
        "<select id=\"{0}\" name=\"{1}\" type=\"text\"{2} value=\"{3}\" onchange=\"FastSorting('{4}')\">{5}</select>",
        this.ID + "_SortBy",
        this.ID + "_SortBy",
        "text",
        string.Empty,
        this.ID,
        this.InitializeSelect());

      output.Write(selectControl);

      output.Write("<table" + this.GetControlAttributes() + ">");
      output.Write("<tr><td class=\"scContentControlMultilistCaption\" >All</td><td class=\"scContentControlMultilistCaption\" >Selected</td></tr>");
      output.Write("<tr>");
      output.Write("<td>");

      output.Write("<ul id=\"" + this.ID + "_sortable1\" class=\"connectedSortable\" >");
      foreach (DictionaryEntry dictionaryEntry in unselected)
      {
        var obj = dictionaryEntry.Value as Item;
        if (obj != null)
        {
          output.Write("<li class=\"ui-state-default\" value=\"" + this.FormatValue(obj) + "\">" + obj.DisplayName + "</li>");
        }
      }

      output.Write("</ul>");
      output.Write("</td>");
      output.Write("<td>");
      output.Write("<ul id=\"" + this.ID + "_sortable2\" class=\"connectedSortable\" >");
      foreach (object t in selected)
      {
        var obj1 = t as Item;
        if (obj1 != null)
        {
          output.Write("<li class=\"ui-state-default\" value=\"" + this.FormatValue(obj1) + "\">" + obj1.DisplayName + "</li>");
        }
      }

      output.Write("</ul>");
      output.Write("</td>");
      output.Write("</tr>");
      output.Write("</table>");
      output.Write("</div>");

      var script = "<script type=\"text/JavaScript\" language=\"javascript\"> jQuery( \"#[0]_sortable1, #[0]_sortable2\" ).sortable({connectWith: \".connectedSortable\" }).disableSelection(); </script>".Replace("[0]", this.ID);
      var script2 = "<script type=\"text/JavaScript\" language=\"javascript\"> jQuery('#[0]_sortable2').bind(\"DOMSubtreeModified\",function(){SetValue('[0]')}); </script>".Replace("[0]", this.ID);

      output.Write(script);
      output.Write(script2);
    }

    /// <summary>
    /// Initializes the select.
    /// </summary>
    /// <returns>Select options</returns>
    private string InitializeSelect()
    {
      var str = string.Empty;
      str += "<option value=\"\"></option>";
      str += "<option value=\"Name\">By Name</option>";
      str += "<option value=\"DateCreated\">By Date Created</option>";
      str += "<option value=\"DateUpdated\">By Date Updated</option>";

      return str;
    }

    /// <summary>
    /// Formats the value.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>formatted value for li</returns>
    private string FormatValue(Item item)
    {
      return "id&{0};datecreated&{1};dateupdated&{2}".FormatWith(item.ID, item.Statistics.Created.ToString("dd.MM.yyyy"), item.Statistics.Updated.ToString("dd.MM.yyyy"));
    }

    /// <summary>
    /// Sets the modified flag.
    /// </summary>
    private void SetModified()
    {
      Sitecore.Context.ClientPage.Modified = true;
    }

    /// <summary>
    /// Gets the selected items.
    /// </summary>
    /// <param name="sources">The sources.</param><param name="selected">The selected.</param><param name="unselected">The unselected.</param><contract><requires name="sources" condition="not null"/></contract>
    private void GetSelectedItems(IEnumerable<Item> sources, out ArrayList selected, out IDictionary unselected)
    {
      Assert.ArgumentNotNull(sources, "sources");
      var listString = new ListString(this.Value);
      unselected = new SortedList(StringComparer.Ordinal);
      selected = new ArrayList(listString.Count);
      foreach (string t in listString)
      {
        selected.Add(t);
      }

      foreach (Item obj in sources)
      {
        string str = obj.ID.ToString();
        int index = listString.IndexOf(str);
        if (index >= 0)
        {
          selected[index] = obj;
        }
        else
        {
          unselected.Add(MainUtil.GetSortKey(obj.Name), obj);
        }
      }
    }
  }
}