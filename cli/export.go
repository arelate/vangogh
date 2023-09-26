package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/packer"
	"net/url"
	"path/filepath"
)

func ExportHandler(_ *url.URL) error {
	aofp, err := vangogh_local_data.GetAbsDir(vangogh_local_data.OutputFiles)
	if err != nil {
		return err
	}
	return Export(aofp)
}

func Export(to string) error {

	ea := nod.NewProgress("exporting metadata...")
	defer ea.End()

	amp, err := vangogh_local_data.GetAbsDir(vangogh_local_data.Metadata)
	if err != nil {
		return ea.EndWithError(err)
	}
	root, _ := filepath.Split(amp)

	if err := packer.Pack(root, amp, to, ea); err != nil {
		return ea.EndWithError(err)
	}

	ea.EndWithResult("done")

	return nil
}
