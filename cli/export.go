package cli

import (
	"archive/tar"
	"compress/gzip"
	"fmt"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"io"
	"net/url"
	"os"
	"path/filepath"
	"time"
)

func ExportHandler(u *url.URL) error {
	return Export("", "")
}

func Export(from, to string) error {

	ea := nod.NewProgress("exporting metadata...")
	defer ea.End()

	efn := fmt.Sprintf(
		"export-%s.tar.gz",
		time.Now().Format("2006-01-02-15-04-05"))

	exportedPath := ""
	if to == "" {
		exportedPath = vangogh_local_data.AbsTempPath(efn)
	} else {
		exportedPath = filepath.Join(to, efn)
	}

	if _, err := os.Stat(exportedPath); os.IsExist(err) {
		return ea.EndWithError(err)
	}

	file, err := os.Create(exportedPath)
	if err != nil {
		return ea.EndWithError(err)
	}
	defer file.Close()

	gw := gzip.NewWriter(file)
	defer gw.Close()

	tw := tar.NewWriter(gw)
	defer tw.Close()

	files := make([]string, 0)

	if from == "" {
		from = vangogh_local_data.AbsMetadataDir()
	}

	if err := filepath.Walk(from, func(f string, fi os.FileInfo, err error) error {
		if fi.IsDir() {
			return nil
		}
		files = append(files, f)
		return nil
	}); err != nil {
		return ea.EndWithError(err)
	}

	ea.TotalInt(len(files))

	for _, f := range files {

		fi, err := os.Stat(f)
		if err != nil {
			return ea.EndWithError(err)
		}

		header, err := tar.FileInfoHeader(fi, f)
		if err != nil {
			return ea.EndWithError(err)
		}

		rp, err := filepath.Rel(vangogh_local_data.Pwd(), f)
		if err != nil {
			return ea.EndWithError(err)
		}

		header.Name = filepath.ToSlash(rp)

		if err := tw.WriteHeader(header); err != nil {
			return ea.EndWithError(err)
		}

		of, err := os.Open(f)
		if err != nil {
			return ea.EndWithError(err)
		}

		if _, err := io.Copy(tw, of); err != nil {
			return ea.EndWithError(err)
		}

		ea.Increment()
	}

	ea.EndWithResult("%s is ready", exportedPath)

	return nil
}
