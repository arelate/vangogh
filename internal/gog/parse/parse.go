package parse

import (
	"errors"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"path"
	"path/filepath"
	"strconv"
	"strings"
)

func Parse(filename, dataDir string) (int, media.Type, error) {

	if !strings.HasPrefix(filename, dataDir) {
		return 0, media.Unknown, errors.New("filename must start with data directory")
	}

	mtid, err := filepath.Rel(dataDir, filename)
	if err != nil {
		return 0, media.Unknown, err
	}

	mtd := filepath.Dir(mtid)
	mt, err := media.Parse(mtd)
	if err != nil {
		return 0, mt, err
	}
	fn, err := filepath.Rel(mtd, mtid)
	if err != nil {
		return 0, mt, err
	}

	if path.Ext(fn) != ".json" {
		return 0, mt, errors.New("filename must have .json extension")
	}

	ids := strings.TrimSuffix(fn, path.Ext(fn))

	id, err := strconv.ParseInt(ids, 0, 64)
	if err != nil {
		return 0, mt, err
	}

	return int(id), mt, nil
}
