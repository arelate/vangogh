package storage

import (
	"io/ioutil"
	"os"
)

func Load(filename string) (bytes []byte, err error) {
	if _, e := os.Stat(filename); e == nil {

		bytes, err = ioutil.ReadFile(filename)
		if err != nil {
			return nil, err
		}
	}

	// TODO: Add generic unmarshal support
	return bytes, nil
}
