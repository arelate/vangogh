package validation

import "encoding/xml"

type File struct {
	XMLName             xml.Name `xml:"file"`
	Name                string   `xml:"name,attr"`
	Available           int      `xml:"available,attr"`
	NotAvailableMessage string   `xml:"notavailablemsg,attr"`
	MD5                 string   `xml:"md5,attr"`
	Chunks              int      `xml:"chunks,attr"`
	Timestamp           string   `xml:"timestamp,attr"`
	TotalSize           int      `xml:"total_size,attr"`
	ValidationChunks    []Chunk  `xml:"chunk"`
}
