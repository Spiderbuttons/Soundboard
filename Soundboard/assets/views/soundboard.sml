<lane orientation="vertical" horizontal-content-alignment="middle" layout="100%" *context={Soundboard}>
    <banner background={@Mods/StardewUI/Sprites/BannerBackground} background-border-thickness="48,0" padding="12" margin="0,8" text="Soundboard" />
    <!-- <frame layout="stretch stretch" background={@Mods/StardewUI/Sprites/ControlBorder} margin="0,0,0,0" padding="32,32"> -->
        <!-- <scrollable> -->
            <!-- <lane orientation="vertical" layout="stretch content">
                <expander layout="stretch content" margin="0,0,0,4" header-padding="0,12" header-background-tint="#11c">
                    <banner *outlet="Header" text="Sound Effects" />
                    <grid layout="stretch content" item-layout="count: 5" item-spacing="16,16" horizontal-item-alignment="middle">
                        <lane *repeat={SoundEffects} *switch={IsPlaying} margin="0,4" padding="2" vertical-content-alignment="middle" +state:playing={IsPlaying} click=|ToggleState()|>
                            <image *case="false" layout="48px" margin="0,0,8,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:PlayButton} />
                            <image *case="true" layout="48px" margin="0,0,8,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:StopButton} />
                            <label layout="stretch content" max-lines="1" text={Id} />
                        </lane>
                    </grid>
                </expander>
            </lane> -->

            <grid layout="stretch content" item-layout="count: 4" item-spacing="64,0" horizontal-item-alignment="middle">
                <frame layout="stretch stretch" background={@Mods/StardewUI/Sprites/ControlBorder} margin="0,0,0,0" padding="32,32">
                    <scrollable>
                    <lane orientation="vertical" layout="stretch content">
                        <lane orientation="vertical" layout="stretch content">
                            <button layout="stretch content" text="Music" margin="0,0,0,16" />
                            <lane orientation="vertical">
                                <lane *repeat={Music} *switch={IsPlaying} +state:playing={IsPlaying} transform-origin="0,0.5" +state:playing:transform="scale: 0.85" margin="0,8" padding="0" vertical-content-alignment="middle" click=|ToggleState()|>
                                    <image *case="false" layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:PlayButton} />
                                    <image *case="true" layout="36px" margin="0,0,4,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:StopButton} />
                                    <label layout="stretch content" max-lines="1" text={Id} />
                                </lane>
                            </lane>
                        </lane>
                    </lane>
                    </scrollable>
                </frame>
                <frame layout="stretch stretch" background={@Mods/StardewUI/Sprites/ControlBorder} margin="0,0,0,0" padding="32,32">
                    <scrollable>
                    <lane orientation="vertical" layout="stretch content">
                        <lane orientation="vertical" layout="stretch content">
                            <button layout="stretch content" text="Sound Effects" margin="0,0,0,16" />
                            <lane orientation="vertical">
                                <lane *repeat={SoundEffects} *switch={IsPlaying} margin="0,4" padding="2" vertical-content-alignment="middle" click=|ToggleState()|>
                                    <image *case="false" layout="48px" margin="0,0,8,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:PlayButton} />
                                    <image *case="true" layout="48px" margin="0,0,8,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:StopButton} />
                                    <label layout="stretch content" max-lines="1" text={Id} />
                                </lane>
                            </lane>
                        </lane>
                    </lane>
                    </scrollable>
                </frame>
                <frame layout="stretch stretch" background={@Mods/StardewUI/Sprites/ControlBorder} margin="0,0,0,0" padding="32,32">
                    <scrollable>
                    <lane orientation="vertical" layout="stretch content">
                        <lane orientation="vertical" layout="stretch content">
                            <button layout="stretch content" text="Ambient" margin="0,0,0,16" />
                            <lane orientation="vertical">
                                <lane *repeat={Ambient} *switch={IsPlaying} margin="0,4" padding="2" vertical-content-alignment="middle" click=|ToggleState()|>
                                    <image *case="false" layout="48px" margin="0,0,8,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:PlayButton} />
                                    <image *case="true" layout="48px" margin="0,0,8,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:StopButton} />
                                    <label layout="stretch content" max-lines="1" text={Id} />
                                </lane>
                            </lane>
                        </lane>
                    </lane>
                    </scrollable>
                </frame>
                <frame layout="stretch stretch" background={@Mods/StardewUI/Sprites/ControlBorder} margin="0,0,0,0" padding="32,32">
                    <scrollable>
                    <lane orientation="vertical" layout="stretch content">
                        <lane orientation="vertical" layout="stretch content">
                            <button layout="stretch content" text="Footsteps" margin="0,0,0,16" />
                            <lane orientation="vertical">
                                <lane *repeat={Footsteps} *switch={IsPlaying} margin="0,4" padding="2" vertical-content-alignment="middle" click=|ToggleState()|>
                                    <image *case="false" layout="48px" margin="0,0,8,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:PlayButton} />
                                    <image *case="true" layout="48px" margin="0,0,8,0" sprite={@Spiderbuttons.Soundboard/Sprites/UI:StopButton} />
                                    <label layout="stretch content" max-lines="1" text={Id} />
                                </lane>
                            </lane>
                        </lane>
                    </lane>
                    </scrollable>
                </frame>
            </grid>
        <!-- </scrollable> -->
    <!-- </frame> -->
</lane>