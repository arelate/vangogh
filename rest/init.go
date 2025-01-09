package rest

import (
	"crypto/sha256"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/middleware"
)

const (
	AdminRole  = "admin"
	SharedRole = "shared"

	SearchResultsLimit = 60 // divisible by 2,3,4,5,6
)

var (
	operatingSystems []vangogh_local_data.OperatingSystem
	langCodes        []string
	noPatches        bool

	rdx kevlar.ReadableRedux
)

func SetDefaultDownloadsFilters(
	os []vangogh_local_data.OperatingSystem,
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

func Init() error {

	var err error
	properties := vangogh_local_data.AllProperties()
	//used by get_downloads
	properties = append(properties, vangogh_local_data.NativeLanguageNameProperty)
	rdx, err = vangogh_local_data.NewReduxReader(properties...)
	return err
}
