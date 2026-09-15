#r "../_lib/Fornax.Core.dll"
#load "layout.fsx"

open Html

let socialLink (social: Profileloader.SocialLink) =
    a [Class "social-link"; Href social.url; Target "_blank"; Rel "noopener"] [
        i [Class social.icon] []
    ]

let projectCard (project: Profileloader.Project) =
    div [Class "column is-4"] [
        div [Class "card project-card"] [
            div [Class "card-content"] [
                p [Class "title is-5"] [!! project.name]
                p [Class "content project-description"] [!! project.description]
                div [Class "tags"] [
                    for tag in project.tags do
                        yield span [Class "tag"] [!! tag]
                ]
                div [Class "project-links"] [
                    for l in Option.toList project.link do
                        yield a [Href l; Target "_blank"; Rel "noopener"] [!! "Visit"]
                    for l in Option.toList project.sourceLink do
                        yield a [Href l; Target "_blank"; Rel "noopener"] [!! "Source"]
                ]
            ]
        ]
    ]

let articleEntry (post: Postloader.Post) =
    li [Class "article-entry"] [
        a [Class "article-entry-title"; Href post.link] [!! post.title]
        span [Class "article-entry-date"] [!! (Layout.published post)]
    ]

let jobEntry (job: Profileloader.Job) =
    div [Class "timeline-item"] [
        div [Class "timeline-item-header"] [
            p [Class "timeline-item-role"] [!! (sprintf "%s · %s" job.role job.company)]
            span [Class "timeline-item-period"] [!! job.period]
        ]
        p [Class "timeline-item-description"] [!! job.description]
    ]

let ossEntry (oss: Profileloader.OssContribution) =
    li [Class "oss-entry"] [
        a [Class "oss-entry-repo"; Href oss.link; Target "_blank"; Rel "noopener"] [!! oss.repo]
        p [Class "oss-entry-description"] [!! oss.description]
    ]

let generate' (ctx: SiteContents) (_: string) =
    let posts =
        ctx.TryGetValues<Postloader.Post> ()
        |> Option.defaultValue Seq.empty
        |> Seq.sortByDescending Layout.published
        |> Seq.toList

    let emptyProfile: Profileloader.Profile =
        { name = ""
          role = ""
          tagline = ""
          bio = ""
          socials = []
          projects = []
          experience = []
          ossContributions = [] }

    let profile =
        ctx.TryGetValue<Profileloader.Profile> ()
        |> Option.defaultValue emptyProfile

    [ "index.html",
      Layout.layout ctx "Home" [
          section [Class "hero-section"] [
              div [Class "container"] [
                  img [Class "hero-avatar"; Src "/images/avatar.jpg"; Alt profile.name]
                  h1 [Class "hero-name"] [!! profile.name]
                  p [Class "hero-role"] [!! profile.role]
                  p [Class "hero-tagline"] [!! profile.tagline]
                  div [Class "hero-socials"] (profile.socials |> List.map socialLink)
              ]
          ]

          section [Class "page-section"] [
              div [Class "container"] [
                  p [Class "bio-text"] [!! profile.bio]
              ]
          ]

          section [Class "page-section"] [
              div [Class "container"] [
                  h2 [Class "section-title"] [!! "Projects"]
                  div [Class "columns is-multiline"] (profile.projects |> List.map projectCard)
              ]
          ]

          section [Class "page-section"] [
              div [Class "container"] [
                  h2 [Class "section-title"] [!! "Articles"]
                  if List.isEmpty posts then
                      p [Class "empty-note"] [!! "No articles yet."]
                  else
                      ul [Class "article-list"] (posts |> List.map articleEntry)
              ]
          ]

          section [Class "page-section"] [
              div [Class "container"] [
                  h2 [Class "section-title"] [!! "Work Experience"]
                  div [Class "timeline"] (profile.experience |> List.map jobEntry)
              ]
          ]

          section [Class "page-section"] [
              div [Class "container"] [
                  h2 [Class "section-title"] [!! "OSS Contributions"]
                  ul [Class "oss-list"] (profile.ossContributions |> List.map ossEntry)
              ]
          ]
      ]
      |> Layout.render ctx ]

let generate (ctx: SiteContents) (projectRoot: string) (page: string) = generate' ctx page
