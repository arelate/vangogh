package internal

import (
	"encoding/gob"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_types"
	"github.com/boggydigital/kvas"
	"path"
)

type Stash struct {
	keyValue *kvas.ValueSet
	memories map[string]string
}

func NewStash(pt vangogh_types.ProductType, mt gog_types.Media, property string) (*Stash, error) {
	dst := path.Join("metadata/_memories", pt.String(), mt.String())

	kvStash, err := kvas.NewJsonLocal(dst)
	if err != nil {
		return nil, err
	}

	memReadCloser, err := kvStash.Get(property)
	if err != nil {
		return nil, err
	}

	defer memReadCloser.Close()

	var memories map[string]string

	if err := gob.NewDecoder(memReadCloser).Decode(&memories); err != nil {
		return nil, err
	}

	return &Stash{
		keyValue: kvStash,
		memories: memories,
	}, nil
}

func (stash *Stash) Memorize(id string, value string) error {
	return nil
}

func (stash *Stash) Recall(id string) (string, error) {
	return "", nil
}
