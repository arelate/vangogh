package compton_pages

import (
	"github.com/arelate/southern_light/github_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/pathways"
)

func WineBinaries() compton.PageElement {

	title := "WINE binaries"

	p := compton.Page(title)

	p.SetAttribute("style", "--c-rep:var(--c-background)")

	pageStack := compton.FlexItems(p, direction.Column)
	p.Append(pageStack)

	titleHeading := compton.HeadingText(title, 1)
	pageStack.Append(compton.FICenter(p, titleHeading))

	operatingSystems := []vangogh_integration.OperatingSystem{vangogh_integration.Windows, vangogh_integration.MacOS, vangogh_integration.Linux}

	gitHubReleasesDir, err := pathways.GetAbsRelDir(vangogh_integration.GitHubReleases)
	if err != nil {
		p.Error(err)
		return p
	}

	kvGitHubReleases, err := kevlar.New(gitHubReleasesDir, kevlar.JsonExt)
	if err != nil {
		p.Error(err)
		return p
	}

	for _, operatingSystem := range operatingSystems {

		pageStack.Append(compton.SectionDivider(p, compton.Text(operatingSystem.String())))

		for _, binary := range vangogh_integration.OsWineBinaries[operatingSystem] {

			binaryPropertyTitle := "Binary"
			binaryTitle := binary.Title
			if binaryTitle == "" {
				binaryPropertyTitle = "GitHub Repo"
				binaryTitle = binary.GitHubOwnerRepo
			}

			binaryVersion := binary.Version
			if binaryVersion == "" {
				latestRelease, err := github_integration.GetLatestRelease(binary.GitHubOwnerRepo, kvGitHubReleases)
				if err != nil {
					p.Error(err)
					return p
				}
				binaryVersion = latestRelease.TagName
			}

			link := compton.A("/api/latest-wine-binary?os=" + operatingSystem.String() + "&title=" + binaryTitle)
			pageStack.Append(link)

			linkFrow := compton.Frow(p).FontSize(size.Small)
			link.Append(compton.FICenter(p, linkFrow))

			linkFrow.PropVal(binaryPropertyTitle, binaryTitle)
			linkFrow.PropVal("Latest Version", binaryVersion)

		}

	}

	return p
}
