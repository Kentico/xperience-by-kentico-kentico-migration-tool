# Migrate pages to widgets

To create custom migrations from pages to widgets:

1. Create a custom director class.
2. Register the class in the project.

## Director class

> :warning: The target page (with an editable area) and any Page Builder components used in the migration need to be present in the system before you migrate content.

1. Create a custom director class that inherits from the `ContentItemDirectorBase` class.
2. Override the `Direct(source, action)` method.

   1. Ensure that the target page has a page template if the source page uses a page template.

      `code block`

   2. Identify components you want to migrate as widgets and use the `action.AsWidget()` action.

      - Specify the location where the widget will be located.

        `code block explain options.Location note that Ancestor uses negative indexing`

      - Specify the properties of the widget

        `options.Properties.Fill itemProps`

## Register the custom migration

To register the custom migration, call...
