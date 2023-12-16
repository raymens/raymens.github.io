#r "../_lib/Fornax.Core.dll"

type SiteInfo = {
    title: string
    description: string
    postPageSize: int
}

let loader (projectRoot: string) (siteContent: SiteContents) =
    let siteInfo =
        { title = "Raymens Blog";
          description = "South is relative."
          postPageSize = 5 }
    siteContent.Add(siteInfo)

    siteContent
