#r "../_lib/Fornax.Core.dll"

/// Data for the portfolio / CV homepage.
/// Replace the sample entries below with your real projects, work history and OSS contributions.

type SocialLink = {
    label: string
    url: string
    icon: string
}

type Project = {
    name: string
    description: string
    tags: string list
    link: string option
    sourceLink: string option
}

type Job = {
    role: string
    company: string
    period: string
    description: string
}

type OssContribution = {
    repo: string
    description: string
    link: string
}

type Profile = {
    name: string
    role: string
    tagline: string
    bio: string
    socials: SocialLink list
    projects: Project list
    experience: Job list
    ossContributions: OssContribution list
}

let profile: Profile = {
    name = "Raymen"
    role = "Software Engineer"
    tagline = "I build reliable software, mostly in F#, and write about it."
    bio =
        "I'm a software engineer with a focus on functional programming and pragmatic system design. "
        + "This page collects the projects I've built, the articles I've written and where I've worked."
    socials = [
        { label = "GitHub"; url = "https://github.com/raymens"; icon = "fa fa-github" }
        { label = "Twitter / X"; url = "https://twitter.com/raymens77"; icon = "fa fa-twitter" }
    ]
    projects = [
        { name = "Project One"
          description = "A short description of this project: what it does and why it's interesting."
          tags = [ "F#"; "Web" ]
          link = None
          sourceLink = None }
        { name = "Project Two"
          description = "A short description of this project: what it does and why it's interesting."
          tags = [ "TypeScript"; "Tooling" ]
          link = None
          sourceLink = None }
        { name = "Project Three"
          description = "A short description of this project: what it does and why it's interesting."
          tags = [ "C#"; "Cloud" ]
          link = None
          sourceLink = None }
    ]
    experience = [
        { role = "Software Engineer"
          company = "Company Name"
          period = "20XX — Present"
          description = "What you did and the impact you had in this role." }
        { role = "Software Engineer"
          company = "Previous Company"
          period = "20XX — 20XX"
          description = "What you did and the impact you had in this role." }
    ]
    ossContributions = [
        { repo = "raymens/raymens.github.io"
          description = "Personal blog and portfolio, built with Fornax (F#)."
          link = "https://github.com/raymens/raymens.github.io" }
    ]
}

let loader (projectRoot: string) (siteContent: SiteContents) =
    siteContent.Add(profile)
    siteContent
