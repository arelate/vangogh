package storage

import (
	"io/ioutil"
	"os"
)

func Load(filename string) (data []byte, err error) {
	data = nil
	if _, e := os.Stat(filename); e == nil {

		data, err = ioutil.ReadFile(filename)
		if err != nil {
			return data, err
		}
	}
	// TODO: Add generic unmarshal support
	return data, nil
}
