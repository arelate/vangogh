package compton_pages

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/southern_light/wine_integration"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
)

func Binaries() compton.PageElement {

	title := "Binaries"

	p := compton.Page(title)

	p.SetAttribute("style", "--c-rep:var(--c-background)")

	pageStack := compton.FlexItems(p, direction.Column)
	p.Append(pageStack)

	appNavLinks := compton_fragments.AppNavLinks(p, "")
	pageStack.Append(compton.FICenter(p, appNavLinks))

	titleHeading := compton.HeadingText(title, 1)
	pageStack.Append(compton.FICenter(p, titleHeading))

	operatingSystems := []vangogh_integration.OperatingSystem{vangogh_integration.Windows, vangogh_integration.MacOS, vangogh_integration.Linux}

	gitHubReleasesDir := vangogh_integration.Pwd.AbsRelDirPath(vangogh_integration.GitHubReleases, vangogh_integration.Metadata)

	kvGitHubReleases, err := kevlar.New(gitHubReleasesDir, kevlar.JsonExt)
	if err != nil {
		p.Error(err)
		return p
	}

	for _, operatingSystem := range operatingSystems {

		pageStack.Append(operatingSystemHeading(p, operatingSystem))

		for _, binary := range wine_integration.OsWineBinaries {

			if binary.OS != operatingSystem {
				continue
			}

			//binaryTitleName := "Title"
			binaryTitle := binary.Title
			if binaryTitle == "" {
				//binaryTitleName = "Repo"
				binaryTitle = binary.GitHubOwnerRepo
			}

			var binaryVersion string
			binaryVersion, err = binary.GetVersion(kvGitHubReleases)
			if err != nil {
				p.Error(err)
				return p
			}

			link := compton.A("/wine-binary-file?os=" + operatingSystem.String() + "&title=" + binaryTitle)
			pageStack.Append(link)

			linkFrow := compton.Frow(p).FontSize(size.Small)
			link.Append(compton.FICenter(p, linkFrow))

			linkFrow.Heading(binaryTitle)
			if binaryVersion != "" {
				linkFrow.PropVal("Version", binaryVersion)
			}

		}

		link := compton.A("/steamcmd-binary-file?os=" + operatingSystem.String())
		linkFrow := compton.Frow(p).FontSize(size.Small)
		link.Append(compton.FICenter(p, linkFrow))
		linkFrow.Heading("SteamCMD")
		pageStack.Append(link)
	}

	pageStack.Append(compton.Br(), compton.FICenter(p, compton_fragments.GitHubLink(p), compton_fragments.LogoutLink(p)))

	return p
}
