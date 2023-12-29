package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/packer"
	"github.com/boggydigital/pathology"
	"net/url"
)

func ExportHandler(_ *url.URL) error {
	aofp, err := pathology.GetAbsDir(vangogh_local_data.Output)
	if err != nil {
		return err
	}
	return Export(aofp)
}

func Export(to string) error {

	ea := nod.NewProgress("exporting metadata...")
	defer ea.End()

	amp, err := pathology.GetAbsDir(vangogh_local_data.Metadata)
	if err != nil {
		return ea.EndWithError(err)
	}

	if err := packer.Pack(amp, to, ea); err != nil {
		return ea.EndWithError(err)
	}

	ea.EndWithResult("done")

	return nil
}
