package rest

import (
	"crypto/sha256"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/middleware"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
)

const (
	AdminRole  = "admin"
	SharedRole = "shared"

	SearchResultsLimit = 60 // divisible by 2,3,4,5,6
)

var (
	operatingSystems []vangogh_integration.OperatingSystem
	langCodes        []string
	noPatches        bool

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

func SetUsername(role, u string) {
	middleware.SetUsername(role, sha256.Sum256([]byte(u)))
}

func SetPassword(role, p string) {
	middleware.SetPassword(role, sha256.Sum256([]byte(p)))
}

func Init(layout vangogh_integration.DownloadsLayout) error {

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	downloadsLayout = layout

	rdx, err = redux.NewReader(reduxDir, vangogh_integration.ReduxProperties()...)
	return err
}
