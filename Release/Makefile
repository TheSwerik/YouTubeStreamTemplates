OPTDIR = $(DESTDIR)/opt
NAME = YouTubeStreamTemplates

install:
		@echo $(NAME)
		find . -type f ! -path "./Makefile" ! -path "./$(NAME).desktop" -exec install -Dm 755 "{}" "$(OPTDIR)/$(NAME)/{}" \;
		export PATH=$$PATH:/opt/$(NAME)
		@if [ -d "/usr/share/applications/" ]; then\
			cp $(NAME).desktop /usr/share/applications/$(NAME).desktop;\
		fi
		@echo "Icon=/opt/$(APP)/icon"

uninstall:
		rm -r $(OPTDIR)/$(NAME)
		@if [ -d "/usr/share/applications/" ]; then\
			rm -r /usr/share/applications/$(NAME).desktop;\
		fi
		@echo "REMOVE PATH"