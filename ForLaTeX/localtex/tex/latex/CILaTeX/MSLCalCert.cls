% Format for MSL calibration reports
% 12 November 2003, B. D. Hall
% Revised to meet new MSL requirements March 2013
% Removed Industrial Research, March 2014
% Updated March 2022, by Chris Young
% Added [hidelinks] 1 June 2023, by Lenice Evergreen
\NeedsTeXFormat{LaTeX2e}
\ProvidesClass{MSLCalCert}[2020/03/20 Format for MSL calibration reports]

\LoadClass[12pt]{article}

% Default cover can be changed by passing an option
\newcommand{\coverimage}{pictures/Report Covers 2019 vCIPM_IANZ}

\DeclareOption{CIPM}
    {\renewcommand{\coverimage}{pictures/Report Covers 2019 vCIPM_IANZ}}

\DeclareOption{IANZ}
    {\renewcommand{\coverimage}{pictures/Report Covers 2019 vIANZ}}

\DeclareOption{unendorsed}
	{\renewcommand{\coverimage}{pictures/Report Covers 2019 vUnendorsed}}	

\DeclareOption*{
  \PackageError{MSLCalCert}{Unknown option '\CurrentOption'}
}

\ProcessOptions\relax

\RequirePackage{vmargin}
\RequirePackage{parskip}
\RequirePackage{fancyhdr}
\RequirePackage{lastpage}
\RequirePackage{xspace}

\RequirePackage{graphicx}	
\usepackage{tikz}

\usepackage[UKenglish]{babel}

\usepackage{times}  % Need this or font looks funny
\usepackage[scaled]{helvet}
\usepackage[T1]{fontenc}

\usepackage{upgreek}
\usepackage{amsmath}
\usepackage{amssymb}
\usepackage{siunitx}


\usepackage[hidelinks]{hyperref}

\usepackage{multirow}   % Allows table column entries that span rows
\usepackage{array}      % Extends the tabular environment
\usepackage{dcolumn}    % Allows allignment on decimal points

\usepackage{changepage}

\usepackage{setspace}

% Set the date format to: day month year
\usepackage[UKenglish,cleanlook]{isodate}
 %-------------------------------------------------------------------
 % vmargin setup (Doesn't seem to bare any relation to documentation!)

 \setpapersize{A4} % Default is A4 (210 x 297)
 %% \setmargins{leftmar}{topmar}{textwidth}{textheight}
 %%     {headheight}{headsep}{footheight}{footskip}
 \setmargins{25mm}{10mm}{160mm}{230mm}
      {20mm}{10mm}{5mm}{10mm}

 %% \setmarginsrb{leftmargin}{topmargin}{rightmargin}{bottommargin}%
 %%     {headheight}{headsep}{footheight}{footskip}
 %% \setmarginsrb{25mm}{25mm}{25mm}{10mm}%
 %%     {5mm}{15mm}{25mm}{10mm}

 \pagestyle{fancyplain}
 %\raggedright
 \tolerance=1
\emergencystretch=\maxdimen
\hyphenpenalty=10000
\hbadness=10000
% Spacing for the report body
% \singlespacing
 \onehalfspacing
% \doublespacing

 \setcounter{secnumdepth}{-1} % No section numbers

% Fonts!!
% Helvetica is hard to distinguish from Arial, which is not in the public domain.
% So I am using Helvetica (phv).
% The default normal text is to be 11-pt Roman. In that case, LaTeX (ref, WikiBooks)
%   \HUGE ~= 23 pt
%   \huge ~= 19 pt
%   \LARGE ~= 16 pt
%   \Large ~= 14 pt
%   \large ~= 12 pt
%   \normal ~= 11 pt
%   \small ~= 10 pt
%   \footnotesize ~= 9 pt
%   \scriptsiize ~= 8.5 pt
%   \tiny ~= 7 pt
%
% The third parameter is the fontseries. `bc` stands for bold condensed
% There are other more condensed width classes but they do not seem to
% be available in standard Helvetica (see Using common PostScript
% fonts with LaTeX, Table 3).
 \newcommand{\mslboldarialnarrow}{\usefont{T1}{phv}{bc}{n}}
 \newcommand{\mslarial}{\usefont{T1}{phv}{m}{n}}
 \newcommand{\mslboldarialsnarrowlanted}{\usefont{T1}{phv}{bc}{sl}}

 %-------------------------------------------------------------------
 \def\thispageof{Page~\thepage~of~\pageref{LastPage}}
 \def\reportnumber#1{\gdef\@reportnumber{#1\xspace}}
 \def\fileref#1{\gdef\@fileref{#1\xspace}}
 \def\@reference{Report No.\xspace \@reportnumber, \@date\xspace}
 \def\title#1{\gdef\@title{#1}\xspace}
 \def\serial#1{\gdef\@serial{#1}\xspace}
 \def\MSL{Measurement Standards Laboratory of New Zealand\xspace}
 \def\CI{Callaghan Innovation\xspace}

% % This bit is no longer used (printed cover sheets are available)
% \def\StandardsAct{
%    Established under the Measurement Standards Act 1992 and the
%    National Standards Regulations 2019 to provide for uniform 
%    measurement of physical quantities throughout New Zealand. 
%    
%    All results quoted in this report are directly traceable to the 
%    national measurement standards held by the Measurement Standards
%    Laboratory of New Zealand (MSL). MSL is New Zealand's national 
%    metrology institute and operates within Callaghan Innovation.
% }
%
% \def\@titlefoot{
%  \begin{flushleft}
%    % The next line allows left-margin indenting
%    \begin{trivlist}\leftskip=5mm
%        \item{\mslarial\footnotesize ISSUED BY THE\par}
%        \vspace{.5\baselineskip}
%        \item{\mslarial\large \MSL \par}
%        \vspace{.5\baselineskip}
%        \item\scriptsize{\mslarial\StandardsAct}
%    \end{trivlist}
%  \end{flushleft}}

 %-------------------------------------------------------------------
 % Note that this not only makes the title page, it also
 % writes the report title on the following page, as required.
 \def\maketitlepage{

	\thispagestyle{empty}   % Header and footer are off
    \begin{singlespace}     % Title page needs single spacing
	
	% The positioning is tweaked for the images we have
	\tikz[remember picture,overlay] \node[opacity=1,inner sep=0pt] at (.5\textwidth,-.55\textheight){
		\includegraphics[width=\paperwidth,height=\paperheight]
			{\coverimage}
	};

    % The Page 1 of x and file reference need to go up to
    % somewhere near the header height, but LaTeX will not
    % allow this. So, I need to change the margins for this
    % page only (reset below)
    %
    % The \changepage arguments are added to current settings
    % parameters: {textheight}{textwidth}{even-side margin}{odd-side margin}
    %   {column sep}{topmargin}{headheight}{headsep}{footskip}
    %
    % This doesn't quite work, not sure what is happening, but it's OK
    % Values are similar used with \setmargins above
    \changepage{}{}{}{}{}{}{}{-20mm}{}

    {\mslboldarialnarrow
        \thispageof\\
         \@fileref\par
    }

    \vspace{70mm}

    \begin{minipage}[c]{142mm}
        {\mslboldarialnarrow\LARGE{\@title{} }\\}
        {\mslboldarialnarrow\LARGE{\@serial{} } \par}
        \vspace{5mm}
        {\mslboldarialnarrow\Large{\@reference} \par}
    \end{minipage}

    \vspace*{\stretch{1}}

% Removed because printed cover sheets are available
%    \begin{minipage}{142mm}
%        \@titlefoot
%    \end{minipage}
%
%    \vspace{0mm} % A positive value will shift the titlefoot upwards

    \clearpage

    % Start the next page by writing the report title
    \vspace*{4\baselineskip}
    {\mslboldarialnarrow\Large{{\@title}\par} }  % Following page needs the title as first element
    \vspace{2\parskip}
    \changepage{}{}{}{}{}{}{}{20mm}{}
    \end{singlespace}
 }

%-------------------------------------------------------------------
% Set up the fancyhdr header and footer
 \renewcommand{\headrulewidth}{0pt}  % Stop having a line ruled by fancyhdr
 \lhead{
    \fancyplain{} {%
        \small{\centering\textbf{\MSL} \\ % Perhaps later: {\MSL, \CI}
        \vspace{.5\baselineskip}
        \@reference}
    }
 }
 \cfoot{
    \fancyplain{} {%
        \small\textit{This report may not be reproduced, except in full, without the written consent of the\\
        Chief Metrologist, Measurement Standards Laboratory of New Zealand}
    }
 }
 \rhead{ \fancyplain{}{\thispageof} }
 \lfoot{}
 \rfoot{}
%-------------------------------------------------------------------
% See p24 LaTeX Companion for help on \@startsection
\makeatletter
\renewcommand\section{
    \@startsection
            {section}
            {1}
            {\z@}%
            {\parskip}
            {0.1\baselineskip}
            {\mslboldarialnarrow\large}
%            {\normalfont\normalsize\sffamily\bfseries}
    }
\renewcommand\subsection{
    \@startsection
            {subsection}
            {2}
            {\z@}%
            {\parskip}
            {0.1\baselineskip}
            {\mslboldarialnarrow\normal}
%            {\normalfont\normalsize\sffamily}
    }
\renewcommand\subsubsection{
    \@startsection
            {subsubsection}
            {3}
            {\z@}
            {\parskip}
            {0.1\baselineskip}%
            {\mslboldarialsnarrowlanted\normalsize}
%            {\normalfont\normalsize\sffamily\slshape}
    }

\makeatother

%-------------------------------------------------------------------
% Macros for the names of the people who will sign.
% So far, three signatures are assumed
%
\newcommand{\signatureA}[1]{\renewcommand{\signatureA}{#1}}
\newcommand{\signatureB}[1]{\renewcommand{\signatureB}{#1}}
\newcommand{\signatureC}[1]{\renewcommand{\signatureC}{#1}}
\newcommand{\signatureD}[1]{\renewcommand{\signatureD}{#1}}
\newcommand{\signatureE}[1]{\renewcommand{\signatureE}{#1}}

\newcommand{\chiefMetrologist}[1]{
	\renewcommand{\chiefMetrologist}{#1} 
	\newcommand{\chiefMetrologistRole}{Chief Metrologist}   
}%
\newcommand{\chiefMetrologistDelegate}[1]{
	\renewcommand{\chiefMetrologist}{#1} 
	\newcommand{\chiefMetrologistRole}{For the Chief Metrologist}   
}%
\newcommand{\sigA}[3]{
	\renewcommand{\signatureA}{#1} 
       \newcommand{\areaA}{#2}
	\newcommand{\signatureARole}{#3}   
}%
\newcommand{\sigB}[3]{
	\renewcommand{\signatureB}{#1} 
      \newcommand{\areaB}{#2}
	\newcommand{\signatureBRole}{#3}   
}%


\newcommand{\signaturesAB}{%
	\begin{center}
		\vspace*{4\baselineskip}
		\begin{minipage}{\textwidth}
			\begin{tabbing}
			
				\rule{4cm}{.1pt} \hspace{1.5cm} 
					\= \rule{4cm}{.1pt} \hspace{1.5cm} 
						\= \rule{4cm}{.1pt} \\
		
				\signatureA \> \signatureB \> \chiefMetrologist \\
				\areaA \> \areaB \> \chiefMetrologistRole \\
				\signatureARole \> \signatureBRole \> \
                
			\end{tabbing}
		\end{minipage}
	\end{center}
}%

\newcommand{\signaturesABC}{%
	\begin{center}
		\vspace*{4\baselineskip}
		\begin{minipage}{\textwidth}
			\begin{tabbing}
			
				\rule{4cm}{.1pt} \hspace{1.5cm} 
					\= \rule{4cm}{.1pt} \hspace{1.5cm} 
						\= \rule{4cm}{.1pt} \\
		
				\signatureA \> \signatureB \> \signatureC \\[4\baselineskip]
                
				\> \> \rule{4cm}{.1pt} \\

                \> \> \chiefMetrologist \\
				\> \> \chiefMetrologistRole
                
			\end{tabbing}
		\end{minipage}
	\end{center}
}%

\newcommand{\signaturesABCD}{%
	\begin{center}
		\vspace*{4\baselineskip}
		\begin{minipage}{\textwidth}
			\begin{tabbing}
			
				\rule{4cm}{.1pt} \hspace{1.5cm} 
					\= \rule{4cm}{.1pt} \hspace{1.5cm} 
						\= \\
		
				\signatureA \> \signatureB \\[4\baselineskip]

				\rule{4cm}{.1pt} \> \rule{4cm}{.1pt} \> \rule{4cm}{.1pt} \\

                \signatureC \> \signatureD \> \chiefMetrologist \\
				\> \> \chiefMetrologistRole
                
			\end{tabbing}
		\end{minipage}
	\end{center}
}%

\newcommand{\signaturesABCDE}{%
	\begin{center}
		\vspace*{4\baselineskip}
		\begin{minipage}{\textwidth}
			\begin{tabbing}
			
				\rule{4cm}{.1pt} \hspace{1.5cm} 
					\= \rule{4cm}{.1pt} \hspace{1.5cm} 
						\= \rule{4cm}{.1pt} \\
		
				\signatureA \> \signatureB \> \signatureC \\[4\baselineskip]

				\rule{4cm}{.1pt} \> \rule{4cm}{.1pt} \> \rule{4cm}{.1pt} \\

                \signatureD \> \signatureE \> \chiefMetrologist \\
				\> \> \chiefMetrologistRole
                
			\end{tabbing}
		\end{minipage}
	\end{center}
}%

% This is an alternative layout, with worker & checker on the first line
% and the Chief Metrologist on the line below
% 				\rule{6cm}{.1pt} \hspace{30mm} 
% 					\= \rule{6cm}{.1pt} \\
% 					
% 				\firstName \> \secondName \\[4\baselineskip]
% 				\> \rule{6cm}{.1pt} \\
% 				\> \chiefMetrologist \\
% 				\> \chiefMetrologistRole



%-------------------------------------------------------------------
% A simple reference to the GUM
%
\newcommand{\referenceGUM}{
For information about uncertainty terminology, see: 
BIPM, IEC, IFCC, ISO, IUPAC, IUPAP and OIML, 
\textit{``Evaluation of measurement data---Guide to the expression of uncertainty in measurement''}, 
BIPM Joint Committee for Guides in Metrology, Paris, S\`evres, edition 1, JCGM~100:2008, 2008. 
A PDF version is available on-line:

\begin{center}
	\href{
		http://www.bipm.org/utils/common/documents/jcgm/JCGM_100_2008_E.pdf
	}{
		http://www.bipm.org/utils/common/documents/jcgm/JCGM\_100\_2008\_E.pdf
	}
\end{center}
}%
