// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package query

import "golang.org/x/net/html"

func Match(doc *html.Node, match func(node *html.Node) bool) *html.Node {
	var f func(*html.Node) *html.Node
	f = func(n *html.Node) *html.Node {
		if match(n) {
			return n
		}
		for c := n.FirstChild; c != nil; c = c.NextSibling {
			val := f(c)
			if val != nil {
				return val
			}
		}
		return nil
	}
	return f(doc)
}
