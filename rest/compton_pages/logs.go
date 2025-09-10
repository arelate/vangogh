package compton_pages

import (
	"os"
	"slices"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
)

func Logs(rdx redux.Readable) compton.PageElement {

	title := "Logs"

	p := compton.Page(title)

	p.SetAttribute("style", "--c-rep:var(--c-background)")

	pageStack := compton.FlexItems(p, direction.Column)
	p.Append(pageStack)

	appNavLinks := compton_fragments.AppNavLinks(p, "")
	pageStack.Append(compton.FICenter(p, appNavLinks))

	titleHeading := compton.HeadingText(title, 1)
	pageStack.Append(compton.FICenter(p, titleHeading))

	absLogsDir, err := pathways.GetAbsDir(vangogh_integration.Logs)
	if err != nil {
		p.Error(err)
		return p
	}

	ld, err := os.Open(absLogsDir)
	if err != nil {
		p.Error(err)
		return p
	}

	files, err := ld.Readdirnames(-1)
	if err != nil {
		p.Error(err)
		return p
	}

	slices.Sort(files)
	slices.Reverse(files)

	logList := compton.FlexItems(p, direction.Column).RowGap(size.Normal)

	for _, fn := range files {
		logLink := compton.A("/logs?id=" + fn)
		logLink.Append(compton.Fspan(p, fn).TextAlign(align.Center))
		logList.Append(logLink)
	}

	if len(files) == 0 {
		logList.Append(compton.Fspan(p, "No logs found on this server.").
			ForegroundColor(color.RepGray).
			TextAlign(align.Center))
	}

	pageStack.Append(compton.Br(), logList)

	pageStack.Append(compton.Br(), compton_fragments.SyncStatus(p, rdx))

	pageStack.Append(compton.Footer(p, "Bonjour d'Arles", "https://github.com/arelate"))

	return p
}
