package rest

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/perm"
	"github.com/boggydigital/author"
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

	downloadsLayout = layout

	var err error
	rdx, err = redux.NewReader(vangogh_integration.AbsReduxDir(), vangogh_integration.ReduxProperties()...)
	if err != nil {
		return err
	}

	authorDir := vangogh_integration.Pwd.AbsRelDirPath(vangogh_integration.Author, vangogh_integration.Metadata)

	sb, err = author.NewSessionBouncer(authorDir, perm.GetRolesPermissions(), "/login")
	if err != nil {
		return err
	}

	return nil
}
