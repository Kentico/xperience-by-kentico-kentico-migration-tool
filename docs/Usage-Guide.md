# Usage Guide

Below you can find a list of prerequisites for the versions of Kentico supported by this migration tool.

For a full list of content and data that is supported by the migration tool, please see [Supported Data](/docs/Supported-Data.md).

## Source

The migration currently supports the Kentico Xperience 13, Kentico 12 or Kentico 11 as the source instance. See the following sections for compatibility information and limitations of respective versions.

### Kentico Xperience 13

- The source of the migration data must be a Kentico Xperience 13 instance, with [Refresh 5](https://docs.kentico.com/13/release-notes-xperience-13#ReleasenotesXperience13-Ref5), [hotfix 13.0.64](https://devnet.kentico.com/download/hotfixes) or newer applied.
- The development model (Core or MVC 5) does not affect the migration - both are supported.
- The source instance's database and file system must be accessible from the environment where you run the Kentico Migration Tool.
- All features described in this repository are available for migration from Kentico Xperience 13.

[![Kentico Xperience 13 upgrade paths](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-kentico-xperience-13-embedded.jpg)](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-kentico-xperience-13-embedded.jpg)

### Kentico 12 MVC

- The source of the migration data can be any hotfix version of the Kentico 12.
  - If you encounter any issues, it is recommended to update to the latest hotfix.
- Only MVC development model is supported by this tool. Any Portal Engine project that you wish to migrate to Xperience by Kentico needs to be [migrated to MVC](https://www.youtube.com/watch?v=g2oeHU0h1e0) first.
- The source instance's database and file system must be accessible from the environment where you run the this tool.
- This repository describes the migration of the Kentico Xperience 13 feature set, however only features relevant to Kentico 12 MVC are migrated for this version.

[![Kentico Xperience 12 MVC upgrade paths](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-kentico-12-mvc-embedded.jpg)](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-kentico-12-mvc-embedded.jpg)

### Kentico 11 and 12 Portal Engine

- The source of the migration data can be any hotfix version of the Kentico 12 or Kentico 11.
  - If you encounter any issues, it is recommended to update to the latest hotfix.
- The source instance's database and file system must be accessible from the environment where you run the this tool.
- Migration of Page Builder content is not supported. Only structured data of pages is migrated.
  - As a result, [source instance API discovery](/Migration.Toolkit.CLI/README.md#source-instance-api-discovery) is also not available.
- This repository describes the migration of the Kentico Xperience 13 feature set, however only features relevant to Kentico 11 and 12 Portal Engine are migrated for this version.

[![Kentico Xperience Portal Engine upgrade paths](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-kentico-10-12-portal-engine-embedded.jpg)](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-kentico-10-12-portal-engine-embedded.jpg)

## Target

- The Kentico Migration Tool is periodically updated to support migration to the **latest version** of Xperience by Kentico. However, there may be delays between Xperience by Kentico releases and tool updates.
  - See the [README](/README.md#library-version-matrix) for supported releases of Xperience by Kentico.
- The target instance's database and file system must be accessible from the environment where you run this tool.
- The target instance's database must be empty except for data from the source instance created by previous runs of this tool to avoid conflicts and inconsistencies.

## Upgrade paths

The full set of upgrade paths to Xperience by Kentico can be seen below.

[![Full Kentico upgrade paths to Xperience by Kentico](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-full-embedded.jpg)](/images/xperience-by-kentico-migration-toolkit-kentico-migration-tool-full-embedded.jpg)

## Migration paths review

As previously mentioned, there are multiple upgrade paths available, particularly when upgrading a project from Kentico 12 to Xperience by Kentico. This section provides an overview of what to expect from each upgrade variant, outlining their respective benefits and costs. This information should help in your strategic decision-making process.

This table shows, for each upgrade path, expectations customers should have for all the steps required to complete the process.

<table border="1" cellpadding="1" cellspacing="1">
  <thead>
    <tr>
      <th style="width: 30%;">Name</th>
      <th>Build a new application</th>
      <th>Migrate Portal Engine Website</th>
      <th>Upgrade to ASP.NET Core</th>
      <th>Rewrite Data Access</th>
      <th>Update to Xperience by Kentico Page Builder</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>
        Kentico 11 -><br />
        Xperience by Kentico
      </td>
      <td>✔</td>
      <td></td>
      <td></td>
      <td></td>
      <td></td>
    </tr>
    <tr>
      <td>
        Kentico 12 Portal Engine -><br />
        Xperience by Kentico
      </td>
      <td>✔</td>
      <td></td>
      <td></td>
      <td></td>
      <td></td>
    </tr>
    <tr>
      <td>
        Kentico 10, Kentico 11 -><br />
        Kentico 12 Portal Engine -><br />
        Kentico 12 MVC -><br />
        Kentico Xperience 13 -><br />
        Xperience by Kentico
      </td>
      <td></td>
      <td>✔</td>
      <td>✔</td>
      <td>✔</td>
      <td>✔</td>
    </tr>
    <tr>
      <td>
        Kentico 12 MVC -><br />
        Xperience by Kentico
      </td>
      <td></td>
      <td></td>
      <td>✔</td>
      <td>✔</td>
      <td>✔</td>
    </tr>
    <tr>
      <td>
        Kentico 12 MVC -><br />
        Kentico Xperience 13 -><br />
        Xperience by Kentico
      </td>
      <td></td>
      <td></td>
      <td>✔</td>
      <td>✔</td>
      <td>✔</td>
    </tr>
    <tr>
      <td>
        Kentico Xperience 13 MVC 5 -><br />
        Xperience by Kentico
      </td>
      <td></td>
      <td></td>
      <td>✔</td>
      <td>✔</td>
      <td>✔</td>
    </tr>
    <tr>
      <td>
        Kentico Xperience 13 ASP.NET Core -><br />
        Xperience by Kentico
      </td>
      <td></td>
      <td></td>
      <td></td>
      <td>✔</td>
      <td>✔</td>
    </tr>
  </tbody>
</table>

The table below compares the benefits and costs of the various upgrade paths to Xperience by Kentico.

<table border="1" cellpadding="1" cellspacing="1">
    <thead>
        <tr>
            <th style="width: 25%;">Name</th>
            <th>Benefits</th>
            <th>Costs</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>Kentico 11, <br>Kentico 12 Portal Engine -><br> Xperience by Kentico</td>
            <td>• Start from “scratch” without throwing away valuable structured content<br> • Very little code might be relevant from this technology and time period, so there’s little loss in building a new application<br> • Marketing teams can re-evaluate their content and marketing strategy without being limited by choices that were made 4-5 years ago.</td>
            <td>• Full application build<br> • If content was not structured, there might be very little that can be migrated<br> • Longer development time with fewer “rest stops” for a team.</td>
        </tr>
        <tr>
            <td>Kentico 10, Kentico 11 -><br> Kentico 12 Portal Engine -><br> Kentico 12 MVC -><br> Kentico Xperience 13 -><br> Xperience by Kentico</td>
            <td>• Many investments made in original solution can be carried forward<br> • Enables many stopping points to define boundaries of a project.<br> • Gives development and marketing teams time to become adjusted to new strategies and technologies<br> • Presentation/design and some page creation workflow for marketers can be preserved exactly, which means less re-training.</td>
            <td>• Might require higher total cost than direct migration to XbyK with all stopping points and intermediate upgrades.<br> • “Lift and shift” mindset. Marketing strategy, design, content modeling, and technology choices from the Portal Engine era might not be relevant today and might lead to a poor implementation and experience in XbyK.</td>
        </tr>
        <tr>
            <td>Kentico 12 MVC -><br> Xperience by Kentico</td>
            <td>• No intermediate KX13 upgrade required<br> • Well-built K12 MVC solutions will become well-built XbyK solutions</td>
            <td>• Team needs to convert entire project from ASP.NET MVC 5 to ASP.NET Core and this needs to be weighed against the cost of writing some functionality from scratch<br> • Many customers/partners did not understand MVC when K12 MVC was released and didn’t design solutions as well as they do today which could lead to unfavorable constraints in XbyK for marketers.<br> Rich-text-heavy / website-focused content will still have to be migrated manually and might block marketers from taking advantage of other channels.<br> “Refurbishing a horse-drawn carriage in the era of electric cars”<br> “Does a customer want a site using carousels, Bootstrap 3, and unstructured content in 2024?”</td>
        </tr>
        <tr>
            <td>Kentico 12 MVC -><br> Kentico Xperience 13 -><br> Xperience by Kentico</td>
            <td>• Benefits are similar to those of K12 PE → K12 MVC → KX13 → XbyK</td>
            <td>• Costs are similar to the costs of K12 PE → K12 MVC → KX13 → XbyK</td>
        </tr>
    </tbody>
</table>

## **Do you need additional assistance?**

While we have made every effort to provide comprehensive documentation, tools, and best practices to assist you in your upgrade process, there may be instances where you would prefer to consult with Kentico experts for additional guidance.

Our consulting team is fully committed to providing help and support to our partners. We offer a [Pre-upgrade audit service](https://www.kentico.com/services/consulting/migration-and-upgrade-assessment) specifically designed to help you avoid pitfalls associated with upgrades. You can find more information about this service [on our website](https://www.kentico.com/services/consulting/migration-and-upgrade-assessment).
