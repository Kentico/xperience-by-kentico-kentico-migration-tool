namespace Migration.Toolkit.Common.Services.Ipc;

public class SourceInstanceDiscoveredInfo
{
    public Dictionary<string, List<EditingFormControlModel>>? WidgetProperties { get; set; }
    public Dictionary<string, List<EditingFormControlModel>>? PageTemplateProperties { get; set; }
    public Dictionary<string, List<EditingFormControlModel>>? SectionProperties { get; set; }
    public string SiteName { get; set; }
}

public class EditingFormControlModel
{
    /// <summary>
    /// Identifier of the form component to be used for editing.
    /// </summary>
    /// <remarks>
    /// The identifier defines a <see cref="T:Kentico.Forms.Web.Mvc.FormComponent`2" /> registered via <see cref="T:Kentico.Forms.Web.Mvc.RegisterFormComponentAttribute" />.
    /// </remarks>
    /// <seealso cref="T:Kentico.Forms.Web.Mvc.RegisterFormComponentAttribute" />
    public string FormComponentIdentifier { get; set; }

    /// <summary>Gets or sets the label of the form component.</summary>
    /// <seealso cref="P:Kentico.Forms.Web.Mvc.FormComponentProperties.Label" />
    public string Label { get; set; }

    /// <summary>Gets or sets the default value of the form component.</summary>
    /// <seealso cref="P:Kentico.Forms.Web.Mvc.FormComponentProperties`1.DefaultValue" />
    public object? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the explanation text of the form component.
    /// </summary>
    /// <seealso cref="P:Kentico.Forms.Web.Mvc.FormComponentProperties.ExplanationText" />
    public string ExplanationText { get; set; }

    /// <summary>Gets or sets the tooltip of the form component.</summary>
    /// <seealso cref="P:Kentico.Forms.Web.Mvc.FormComponentProperties.Tooltip" />
    public string Tooltip { get; set; }

    /// <summary>Gets or sets the order weight of the form component.</summary>
    /// <seealso cref="T:Kentico.Forms.Web.Mvc.EditingComponentOrder" />
    public int? Order { get; set; }
    
    public string PropertyName { get; set; }
}