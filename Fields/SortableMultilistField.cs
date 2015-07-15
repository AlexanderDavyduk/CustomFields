// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortableMultilistField.cs">
//   Copyright (C) 2015 by Alexander Davyduk. All rights reserved.
// </copyright>
// <summary>
//   Defines the SortableTreeList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.SharedSource.CustomFields.Fields
{
  using System.Collections;
  using Sitecore.Data;
  using Sitecore.Data.Fields;
  using Sitecore.Data.Items;

  /// <summary>
  /// Defines Tree list Sortable field
  /// </summary>
  public class SortableMultilistField : DelimitedField
  {
    /// <summary>
    /// Gets the list of target IDs.
    /// </summary>
    /// <value>
    /// The target I ds.
    /// </value>
    public ID[] TargetIDs
    {
      get
      {
        var arrayList = new ArrayList();
        string str = this.Value;
        char[] chArray = new char[1]
        {
          '|'
        };

        foreach (string id in str.Split(chArray))
        {
          if (id.Length > 0 && ID.IsID(id))
            arrayList.Add(ID.Parse(id));
        }

        return arrayList.ToArray(typeof(ID)) as ID[];
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MultilistField"/> class. 
    /// Creates a new <see cref="T:Sitecore.Data.Fields.MultilistField"/> instance.
    /// </summary>
    /// <param name="innerField">
    /// Inner field.
    /// </param>
    public SortableMultilistField(Field innerField)
      : base(innerField, '|')
    {
    }

    /// <summary>
    /// Converts a <see cref="T:Sitecore.Data.Fields.Field"/> to a <see cref="T:Sitecore.Data.Fields.MultilistField"/>.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>
    /// The implicit operator.
    /// </returns>
    public static implicit operator SortableMultilistField(Field field)
    {
      if (field != null)
      {
        return new SortableMultilistField(field);
      }

      return null;
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <returns>
    /// The items.
    /// </returns>
    public Item[] GetItems()
    {
      var arrayList = new ArrayList();
      var database = this.GetDatabase();
      if (database == null)
      {
        return null;
      }

      foreach (var itemId in this.TargetIDs)
      {
        var obj = database.GetItem(itemId);
        if (obj != null)
        {
          arrayList.Add(obj);
        }
      }

      return arrayList.ToArray(typeof(Item)) as Item[];
    }
  }
}