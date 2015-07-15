// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameValueDroplistField.cs" >
//   Copyright (C) 2015 by Alexander Davyduk. All rights reserved.
// </copyright>
// <summary>
//   Defines the NameValueDroplistField class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.SharedSource.CustomFields.Fields
{
  using System.Collections.Specialized;
  using System.Linq;
  using Sitecore.Data.Fields;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Defines NameValueDroplistField class
  /// </summary>
  public class NameValueDroplistField : NameValueListField
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NameValueDroplistField"/> class.
    /// </summary>
    /// <param name="innerField">Inner field.</param>
    public NameValueDroplistField(Field innerField)
      : base(innerField)
    {
    }

    /// <summary>
    /// Names the value droplist field.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns></returns>
    public static implicit operator NameValueDroplistField(Field field)
    {
      return field != null ? new NameValueDroplistField(field) : null;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Sitecore.Data.Fields.CheckboxField" /> is checked.
    /// </summary>
    /// <value>
    ///   <c>true</c> if checked; otherwise, <c>false</c>.
    /// </value>
    public new NameValueCollection NameValues
    {
        get
        {
          var collection = StringUtil.ParseNameValueCollection(this.Value, '&', '=');

          var sortedCollection = new NameValueCollection();

          var items = collection.AllKeys.Where(k => !string.IsNullOrEmpty(k))
                                        .Select(k => new { Item = this.InnerField.Database.GetItem(k), Key = k });

          foreach (var item in items.OrderBy(i => i.Item == null ? 0 : i.Item.Appearance.Sortorder))
          {
            sortedCollection.Add(item.Key, collection[item.Key]);
          }

          return sortedCollection;
        }

        set
        {
            Assert.ArgumentNotNull(value, "value");
            this.Value = StringUtil.NameValuesToString(value, "&");
        }
    }

  }
}