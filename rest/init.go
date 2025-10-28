package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/perm"
	"github.com/boggydigital/author"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
)

const (
	SearchResultsLimit = 60 // divisible by 2,3,4,5,6
)

var (
	operatingSystems []vangogh_integration.OperatingSystem
	langCodes        []string
	noPatches        bool

	sb *author.SessionBouncer

	downloadsLayout vangogh_integration.DownloadsLayout

	rdx redux.Readable
)

func SetDefaultDownloadsFilters(
	os []vangogh_integration.OperatingSystem,
	lc []string,
	np bool) {
	operatingSystems = os
	langCodes = lc
	noPatches = np
}

func Init(layout vangogh_integration.DownloadsLayout) error {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	downloadsLayout = layout

	rdx, err = redux.NewReader(reduxDir, vangogh_integration.ReduxProperties()...)
	if err != nil {
		return err
	}

	authorDir, err := pathways.GetAbsRelDir(vangogh_integration.Author)
	if err != nil {
		return err
	}

	sb, err = author.NewSessionBouncer(authorDir, perm.GetRolesPermissions(), "/login")
	if err != nil {
		return err
	}

	return nil
}
