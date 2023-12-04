using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using CMS.SiteProvider;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using Newtonsoft.Json;


public class ToolkitApiController : Controller
{
    // TODO configure your own secret
    private const string Secret = "secret_used_for_http_posts_to_source_instance_change_to_random_key";

    public ToolkitApiController() { }

    public class ToolkitInfo
    {
        public Dictionary<string, List<EditingFormControlModel>> WidgetProperties { get; set; }
        public Dictionary<string, List<EditingFormControlModel>> PageTemplateProperties { get; set; }
        public Dictionary<string, List<EditingFormControlModel>> SectionProperties { get; set; }

        public string SiteName { get; set; }
    }

    public class BodyModel
    {
        public string Secret { get; set; }
    }

    [HttpPost()]
    public ActionResult Test(BodyModel body)
    {
        if (!HttpContext.Request.IsLocal)
        {
            return new HttpNotFoundResult();
        }

        if (Secret != body?.Secret)
        {
            return new HttpNotFoundResult();
        }

        return ToJsonResult(new
        {
            pong = true
        });
    }

    [HttpPost()]
    public ActionResult GetAllDefinitions(BodyModel body)
    {
        if (!HttpContext.Request.IsLocal)
        {
            return new HttpNotFoundResult();
        }

        if (Secret != body?.Secret)
        {
            return new HttpNotFoundResult();
        }

        var widgetPropertiesResult = new Dictionary<string, List<EditingFormControlModel>>();
        var pageTemplatePropertiesResult = new Dictionary<string, List<EditingFormControlModel>>();
        var sectionPropertiesResult = new Dictionary<string, List<EditingFormControlModel>>();
        var result = new ToolkitInfo
        {
            WidgetProperties = widgetPropertiesResult,
            PageTemplateProperties = pageTemplatePropertiesResult,
            SectionProperties = sectionPropertiesResult,
            SiteName = SiteContext.CurrentSiteName
        };

        var allWidgetDefinitions = new ComponentDefinitionProvider<WidgetDefinition>().GetAll();

        foreach (var widgetDefinition in allWidgetDefinitions)
        {
            var editingFormControlModels = new List<EditingFormControlModel>();
            widgetPropertiesResult.Add(widgetDefinition.Identifier, editingFormControlModels);
            if (widgetDefinition.PropertiesType == null) continue;
            foreach (var propertyInfo in widgetDefinition.PropertiesType.GetProperties())
            {
                var editingComponent = propertyInfo.GetCustomAttribute<EditingComponentAttribute>();
                if (editingComponent != null)
                {
                    var model = new EditingFormControlModel
                    {
                        PropertyName = propertyInfo.Name,
                        Label = editingComponent.Label,
                        DefaultValue = editingComponent.DefaultValue,
                        ExplanationText = editingComponent.ExplanationText,
                        Tooltip = editingComponent.Tooltip,
                        Order = editingComponent.Order,
                        FormComponentIdentifier = editingComponent.FormComponentIdentifier,
                    };
                    editingFormControlModels.Add(model);
                }
            }
        }

        var allPageTemplates = new ComponentDefinitionProvider<PageTemplateDefinition>().GetAll();
        foreach (var pageTemplateDef in allPageTemplates)
        {
            var results = new List<EditingFormControlModel>();
            pageTemplatePropertiesResult.Add(pageTemplateDef.Identifier, results);
            if (pageTemplateDef.PropertiesType == null) continue;
            foreach (var propertyInfo in pageTemplateDef.PropertiesType.GetProperties())
            {
                var editingComponent = propertyInfo.GetCustomAttribute<EditingComponentAttribute>();
                if (editingComponent != null)
                {
                    var model = new EditingFormControlModel
                    {
                        PropertyName = propertyInfo.Name,
                        Label = editingComponent.Label,
                        DefaultValue = editingComponent.DefaultValue,
                        ExplanationText = editingComponent.ExplanationText,
                        Tooltip = editingComponent.Tooltip,
                        Order = editingComponent.Order,
                        FormComponentIdentifier = editingComponent.FormComponentIdentifier,
                    };
                    results.Add(model);
                }
            }
        }

        var allSectionProperties = new ComponentDefinitionProvider<Kentico.PageBuilder.Web.Mvc.SectionDefinition>().GetAll();
        foreach (var definition in allSectionProperties)
        {
            var results = new List<EditingFormControlModel>();
            sectionPropertiesResult.Add(definition.Identifier, results);
            if (definition.PropertiesType == null) continue;
            foreach (var propertyInfo in definition.PropertiesType.GetProperties())
            {
                var editingComponent = propertyInfo.GetCustomAttribute<EditingComponentAttribute>();
                if (editingComponent != null)
                {
                    var model = new EditingFormControlModel
                    {
                        PropertyName = propertyInfo.Name,
                        Label = editingComponent.Label,
                        DefaultValue = editingComponent.DefaultValue,
                        ExplanationText = editingComponent.ExplanationText,
                        Tooltip = editingComponent.Tooltip,
                        Order = editingComponent.Order,
                        FormComponentIdentifier = editingComponent.FormComponentIdentifier,
                    };
                    results.Add(model);
                }
            }
        }

        return ToJsonResult(result);
    }

    [HttpPost()]
    public ActionResult GetAllFormComponents(BodyModel body)
    {
        if (!HttpContext.Request.IsLocal)
        {
            return new HttpNotFoundResult();
        }

        if (Secret != body?.Secret)
        {
            return new HttpNotFoundResult();
        }

        var allFormComponents = new ComponentDefinitionProvider<FormComponentDefinition>().GetAll();

        return ToJsonResult(allFormComponents.Select(x => new
        {
            x.Name,
            x.Identifier,
        }));
    }

    public static ActionResult ToJsonResult(object obj)
    {
        var content = new ContentResult();
        content.Content = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        content.ContentType = "application/json";
        return content;
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
        public object DefaultValue { get; set; }

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
        public int Order { get; set; }

        public string PropertyName { get; set; }
    }
}