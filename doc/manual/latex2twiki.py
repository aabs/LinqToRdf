#!/usr/bin/env python
# -*- coding: ISO-8859-1 -*-
#
# Mike Meylan is hacking this to include more latex commands
# and Malte has added a few things that get rid of his personal latex commands.
#
# This code has been modified by Anthony Miller for handling of inline 
# mathematics and more sophisticated documents.
#
# Original idea from : 
#       Maxime Biais <maxime@biais.org>
#     but has been nearly all rewritten since...
# A good fraction of this code was written by
# Marc Poulhi�s <marc.poulhies@epfl.ch>
#
#    This program is free software; you can redistribute it and/or modify
#    it under the terms of the GNU General Public License as published by
#    the Free Software Foundation; either version 2 of the License, or
#    (at your option) any later version.
#
#    This program is distributed in the hope that it will be useful,
#    but WITHOUT ANY WARRANTY; without even the implied warranty of
#    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#    GNU General Public License for more details.
#
#    You should have received a copy of the GNU General Public License
#    along with this program; if not, write to the Free Software
#    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#
# $Id: latex2twiki.py,v 1.2 2005/07/27 12:40:53 poulhies Exp $

import sys, re

bullet_level=0
enum_level = 0
bdoc = None
end_line = 1
count = 0;
label_list = [];
lines = [];

verbatim_mode = 0
math_mode = 0
eqnarry_mode = 0
label_mode = 0

def dummy():
    pass

def toggle_label():
	global label_mode
	label_mode += 1

def inc_bullet():
	global bullet_level
	bullet_level += 1

def dec_bullet():
	global bullet_level
	bullet_level -= 1

def inc_enum():
	global enum_level
	enum_level += 1
	
def dec_enum():
	global enum_level
	enum_level -= 1
	
def start_doc():
	global bdoc;
	bdoc = 1

def do_not_el():
	global end_line
	end_line = None

def do_el():
	global end_line;
	end_line = 1

def decide_el():
	global end_line
	if bullet_level == 0:
		return "\n"
	else:
		return " "

def decide_math_replace():
	global math_mode
	if math_mode == 1:
		return r"\1"
	else:
		return " "

def decide_math():
	global math_mode
	if math_mode == 1:
		return "<math>"
	else:
		return "</math>"
		
def start_verbatim():
	global verbatim_mode
	verbatim_mode = 1

def end_verbatim():
	global verbatim_mode
	verbatim_mode = 0

def start_eqnarry():
	global eqnarry_mode
	eqarry_mode = 1

def end_eqnarry():
	global eqnarry_mode
	eqnarry_mode = 0

def toggle_math():
	global math_mode
	math_mode = 1 - math_mode

conv_table = { '>':'&gt;',
			   '<':'&lt;'}

def translate_to_html(char):
	global verbatim_mode
	global conv_table
	if verbatim_mode == 0:
		return conv_table[char]
	else:
		return char

NONE = "__@NONE@__"

tr_list2 = [
	(r"\\footnotesize", None, dummy),
	(r"\\begin\{abstract}", None, dummy),
	(r"\\begin\{article}", None, dummy),
	(r"\\end\{abstract}", None, dummy),
	(r"\\end\{article}", None, dummy),
	(r"\\end\{document}", None, dummy),
	(r"\\protect", None, dummy),
	(r"\\small", None, dummy),
	(r"\\func", None, dummy),
	(r"\\begin\{document}", None, start_doc),
	(r"\\cite{(.*?)}", (lambda :r"[[\1]]"), dummy),
#	(r"\\label{(.*?)}", (lambda :r" (\1)"), dummy),
#	(r"\\ref{(.*?)}", (lambda :r"(\1)"), dummy),
	(r"\\citep{(.*?)}", (lambda :r"[[\1]]"), dummy),
	(r"\\citet{(.*?)}", (lambda :r"[[\1]]"), dummy),
	(r"\\emph{(.*?)}", (lambda :r"''\1'' "), dummy),
	(r"\\textit{(.*?)}", (lambda :r"''\1'' "), dummy),
	(r"\\texttt{(.*?)}", (lambda : r"=\1= "), dummy),
	(r"\\text{(.*?)}", (lambda : r"=\1= "), dummy),
	(r"\\textbf{(.*?)}", (lambda : r"'''\1''' "), dummy),

	(r"\\rm{}", (lambda : r"RdfMetal"), dummy),
	(r"\\l2r{}", (lambda : r"LinqToRdf"), dummy),
	(r"\\spq{}", (lambda : r"SPARQL"), dummy),
	(r"\\cs{}", (lambda : r"C#"), dummy),

	(r"\\&", (lambda : r"&amp;"), dummy),

	(r"\\rm\\ ", (lambda : r"RdfMetal "), dummy),
	(r"\\l2r\\ ", (lambda : r"LinqToRdf "), dummy),
	(r"\\spq\\ ", (lambda : r"SPARQL "), dummy),
	(r"\\cs\\ ", (lambda : r"C# "), dummy),

	(r"\\begin{verbatim}", (lambda : "<br><code>"), start_verbatim),
	(r"\\end{verbatim}", (lambda : "</code><br>"), end_verbatim),
	(r"\\begin{itemize}", (lambda : "\n"), inc_bullet),
	(r"\\end{itemize}", None, dec_bullet),
	(r"\\begin{enumerate}", (lambda : "\n"), inc_enum),
	(r"\\end{enumerate}", None, dec_enum),
	(r"\\item (.*?)", (lambda :  (r"#" * enum_level) +(r"\n*" * bullet_level) + r"\1"), dummy),
	(r"\\begin{equation[*]*}", (lambda :"<center><math>"), toggle_math),
	(r"\\end{equation[*]*}", (lambda :"</math></center>"), toggle_math),
	(r"\\\[", (lambda :"<center><math>"), toggle_math),
	(r"\\dfrac", (lambda :r"\\frac"), dummy),
	(r"\\\]", (lambda :"</math></center>"), toggle_math),
	(r"\\begin{eqnarray[*]?}", (lambda :r"<center><math>\\begin{matrix}"), toggle_math),
	(r"\\begin{array[*]?}", (lambda :r"\\begin{matrix}"), toggle_math),
	(r"\\end{eqnarray[*]?}", (lambda :r"\\end{matrix}</math></center>"), toggle_math),
	(r"\\end{array[*]?}", (lambda :r"\\end{matrix}"), toggle_math),
#	(r"(\\begin{.*?})", decide_math_replace, dummy),
#	(r"(\\end{.*?})",decide_math_replace, dummy),
#	(r"~\\ref{([^}]*)}",(lambda : r" ---\1---"),dummy),
	(r"``(.*?)''", (lambda :r'"\1"'), dummy),
	(r"\\subsubsection{(.*?)}", (lambda : r"===\1==="), dummy),
	(r"\\subsection{(.*?)}", (lambda : r"==\1=="), dummy),
	(r"\\section{(.*?)}", (lambda : r"=\1="), dummy),
	(r"\\_", (lambda :"_"), dummy),
	 (r"\\title{(.*)}", (lambda :r"= \1 ="),dummy),
    (r"\\author{(.*)}", (lambda :r"\1"),dummy),
    (r"\\date{(.*)}", (lambda :r"\1"),dummy),
	(r"\\tableofcontents",None, dummy),
	(r"\\null",None, dummy),
	(r"\\newpage",None, dummy),
	(r"\\thispagestyle{.*?}", None, dummy),
	(r"\\maketitle", None, dummy),
	(r"\n$", decide_el, dummy),
#	(r"[^\\]?\{", None, dummy),
#	(r"[^\\]?\}", None, dummy),
	(r"\$(.*?)\$",(lambda :r"<math>\1</math>"),dummy),
	(r"\$",decide_math,toggle_math),
	(r"%.*$",None, dummy),
	(r"\\r{(.*?)}", (lambda : r"\\mathrm{\1}"), dummy),
	(r"\\d ", (lambda : r"\\,\mathrm{d} "), dummy),
	(r"\\i ", (lambda : r"\\mathrm{i} "), dummy),
	(r"\\i\\", (lambda : r"\\mathrm{i}\\"), dummy),
	(r"\\e\^", (lambda : r"\\mathrm{e}^"), dummy),
	(r"\\begin{align[*]?}", (lambda :r"<center><math>\\begin{matrix}"), toggle_math),
	(r"\\end{align[*]?}", (lambda :r"\\end{matrix}</math></center>"), toggle_math),
	(r"\\begin{aligned[*]?}", None, dummy),
	(r"\\end{aligned[*]?}", None, dummy),
	(r"\\begin{subequations[*]?}", None, dummy),
	(r"\\end{subequations[*]?}", None, dummy),
	(r"\\nonumber", None, dummy),
	(r"\\displaystyle", None, dummy),
      (r"\\hdots", None, dummy),
      (r"\{c\}", None, dummy),
      (r"\{cc\}", None, dummy),
      (r"\{ccc\}", None, dummy),
      (r"\{cccc\}", None, dummy),
      (r"\{ccccc\}", None, dummy),
      (r"\{cccccc\}", None, dummy),
      (r"\{ccccccc\}", None, dummy),
      (r"\{cccccccc\}", None, dummy),
	(r"\\dint", (lambda : r"\\int"), dummy),
	(r"\\hspace{.*?}", None, dummy),        
	(r"\\bf", (lambda : r"\\mathbf"), dummy),
	(r"\\ast", (lambda : r"*"), dummy),
	(r"\\tfrac", (lambda : r"\\frac"), dummy),
	(r"\\leqslant", (lambda : r"\\leq"), dummy),
	(r"\\geqslant", (lambda : r"\\geq"), dummy),
	(r"\\notag", None, dummy)


    ]

in_stream  = sys.stdin;
out_stream = sys.stdout

# precompile regular expressions
tr_list3 = map(lambda x: (re.compile(x[0]),x[1],x[2]),tr_list2)

label = re.compile(r"\\label{(.*?)}")
for line in in_stream.readlines():

	if label.search(line):
		mch = label.search(line)
		count +=1
		label_list.append(mch.group(1))
		line = label.sub(r"("+'%d'%count+r")",line)
	for reg in tr_list3:
		if reg[0].search(line):
			reg[2]()
			if reg[1] != None:
				line = reg[0].sub(reg[1](),line)
			else:
				line = reg[0].sub("", line)
	if bdoc != None:
		lines.append(line)
ref = re.compile(r"\\ref{(.*?)}")
for line in lines:
	if ref.search(line):
		list = ref.findall(line)
		for item in list:
			r=re.compile(r"\\ref{" + item + r"}")
			if item in label_list:
				line = r.sub('%d'%(label_list.index(item)+1), line)
	print >> out_stream, line,


